using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Cinemachine;
using PsychoticLab;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using static Cinemachine.AxisState;
using Cursor = UnityEngine.Cursor;

public class PlayerCustomize : NetworkBehaviour
{


    [SerializeField] private GameObject _player;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private PlayerAiming _playerAiming;

    [Header("Variables")]
    [SerializeField] private float _cameraDistance = 3f;
    private ShowInventory _showInventory;
    private Animator _playerAnimator;
    private PlayerControls _playerControls;
    private CinemachineVirtualCamera _vcam;
    public GameObject _tempObject = null;
    
    public Transform[] _allChildObjects;
    public ScriptableObject _allPlayerModules;

    public int[] _allChildObjectsIndex;

    public GameObject UI_Customize;
    public GameObject UI_Inventory;

    public Transform _lookAt;

    [HideInInspector] public Vector3 cameraPositionWhenCustomizing;
    [HideInInspector] public Vector3 playerPositionWhenCustomizing;
    [HideInInspector] public float playerRotationWhenCustomizing;



    public bool isMale = true;



    [Header("Material")]
    public Material mat;

    private void Awake()
    {
        UI_Inventory = GameObject.FindGameObjectWithTag("UI_Inventory");
        UI_Customize = GameObject.FindGameObjectWithTag("UI_Customize");
    }


    void Start()
    {
        _player = gameObject;
        _playerController = _player.GetComponent<PlayerController>();
        _playerAiming = GetComponent<PlayerAiming>();


        _showInventory = GetComponent<ShowInventory>();
        _playerControls = new PlayerControls();
        _playerAnimator = GetComponent<Animator>();
        _allChildObjects = transform.Find("Modular_Characters").GetComponentsInChildren<Transform>();

        


        BuildLists();

        // disable any enabled objects before clear
        if (enabledObjects.Count != 0)
        {
            foreach (GameObject g in enabledObjects)
            {
                g.SetActive(false);
            }
        }

        // clear enabled objects list
        enabledObjects.Clear();

        // set default male character
        
        ActivateItem(male.headAllElements[0]);
        ActivateItem(male.eyebrow[0]);
        ActivateItem(male.facialHair[0]);
        ActivateItem(male.torso[0]);
        ActivateItem(male.arm_Upper_Right[0]);
        ActivateItem(male.arm_Upper_Left[0]);
        ActivateItem(male.arm_Lower_Right[0]);
        ActivateItem(male.arm_Lower_Left[0]);
        ActivateItem(male.hand_Right[0]);
        ActivateItem(male.hand_Left[0]);
        ActivateItem(male.hips[0]);
        ActivateItem(male.leg_Right[0]);
        ActivateItem(male.leg_Left[0]);
        ActivateItem(allGender.all_Hair[0]);



        _playerControls.Enable();

        _playerControls.Customize.Close.started += Close;
        _playerControls.Customize.Close.canceled += Close;

        _playerControls.Customize.MoveCamera.started += MoveCamera;
        _playerControls.Customize.MoveCamera.canceled += MoveCamera;

        _vcam = _player.GetComponent<PlayerController>().GetCamera().GetComponent<CinemachineVirtualCamera>();

        enabled = false;
    }

    private Quaternion _oldCameraRotation;

    public void Initialize()
    {
        UI_Inventory.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        
        _showInventory.enabled = false;


        _playerAnimator.SetBool("isMoving", false);
        _playerAnimator.SetBool("isRunning", false);
        _playerAnimator.SetBool("isOffhanding", false);
        _playerAnimator.SetBool("isFalling", false);


        UI_Customize.SetActive(true);

        _oldCameraRotation = _vcam.transform.rotation;


    }

    [ClientRpc]
    void UpdateCharacterClientRpc(int[] _allChildObjectsIndex)
    {
        if (!IsHost)
        {
            for (int i = 0; i < _allChildObjectsIndex.Length; i++)
            {
                ActivateItem(_allChildObjects[_allChildObjectsIndex[i]].gameObject);
            }
        }
        else
        {
        }
    }

    [ServerRpc]
    void UpdateCharacterServerRpc(int[] _allChildObjectsIndex)
    {
        if (!IsHost) return;
        
        {
            for (int i = 0; i < _allChildObjectsIndex.Length; i++)
            {
                ActivateItem(_allChildObjects[_allChildObjectsIndex[i]].gameObject);
            }
        }
    }


    private void Update()
    {


        if (_player.GetComponent<PlayerController>().isCustomizing)
        {
            float distance = Vector3.Distance(_player.transform.position, playerPositionWhenCustomizing);
            
            if (distance > 0.01f)
            {
                _player.transform.position = Vector3.MoveTowards(_player.transform.position, playerPositionWhenCustomizing, Time.deltaTime * 2f);
                _player.transform.rotation = Quaternion.RotateTowards(_player.transform.rotation, Quaternion.LookRotation(playerPositionWhenCustomizing - _player.transform.position, Vector3.up), Time.deltaTime * 360f);
                _playerAnimator.SetBool("isMoving", true);
            }
            else
            {
                _player.transform.rotation = Quaternion.RotateTowards(_player.transform.rotation, Quaternion.Euler(0, playerRotationWhenCustomizing, 0), Time.deltaTime * 180f);
                _playerAnimator.SetBool("isMoving", false);
            }

            // Prevent camera rotation
            _playerAiming.cameraLookAt.eulerAngles = new Vector3(_playerAiming.yAxis.Value, _playerAiming.xAxis.Value, 0);

        }
    }


    // list of all objects on character

    public List<GameObject> allObjects = new List<GameObject>();

    // list of enabed objects on character
    public List<GameObject> enabledObjects = new List<GameObject>();


    // character object lists
    // male list
    
    public CharacterObjectGroups male;

    // female list
    
    public CharacterObjectGroups female;

    // universal list
    
    public CharacterObjectListsAllGender allGender;

    
    private void BuildLists()
    {
        
        //build out male lists
        BuildList(male.headAllElements, "Male_Head_All_Elements");
        BuildList(male.headNoElements, "Male_Head_No_Elements");
        BuildList(male.eyebrow, "Male_01_Eyebrows");
        BuildList(male.facialHair, "Male_02_FacialHair");
        BuildList(male.torso, "Male_03_Torso");
        BuildList(male.arm_Upper_Right, "Male_04_Arm_Upper_Right");
        BuildList(male.arm_Upper_Left, "Male_05_Arm_Upper_Left");
        BuildList(male.arm_Lower_Right, "Male_06_Arm_Lower_Right");
        BuildList(male.arm_Lower_Left, "Male_07_Arm_Lower_Left");
        BuildList(male.hand_Right, "Male_08_Hand_Right");
        BuildList(male.hand_Left, "Male_09_Hand_Left");
        BuildList(male.hips, "Male_10_Hips");
        BuildList(male.leg_Right, "Male_11_Leg_Right");
        BuildList(male.leg_Left, "Male_12_Leg_Left");

        
        //build out female lists
        BuildList(female.headAllElements, "Female_Head_All_Elements");
        BuildList(female.headNoElements, "Female_Head_No_Elements");
        BuildList(female.eyebrow, "Female_01_Eyebrows");
        BuildList(female.facialHair, "Female_02_FacialHair");
        BuildList(female.torso, "Female_03_Torso");
        BuildList(female.arm_Upper_Right, "Female_04_Arm_Upper_Right");
        BuildList(female.arm_Upper_Left, "Female_05_Arm_Upper_Left");
        BuildList(female.arm_Lower_Right, "Female_06_Arm_Lower_Right");
        BuildList(female.arm_Lower_Left, "Female_07_Arm_Lower_Left");
        BuildList(female.hand_Right, "Female_08_Hand_Right");
        BuildList(female.hand_Left, "Female_09_Hand_Left");
        BuildList(female.hips, "Female_10_Hips");
        BuildList(female.leg_Right, "Female_11_Leg_Right");
        BuildList(female.leg_Left, "Female_12_Leg_Left");

        // build out all gender lists
        BuildList(allGender.all_Hair, "All_01_Hair");
        BuildList(allGender.all_Head_Attachment, "All_02_Head_Attachment");
        BuildList(allGender.headCoverings_Base_Hair, "HeadCoverings_Base_Hair");
        BuildList(allGender.headCoverings_No_FacialHair, "HeadCoverings_No_FacialHair");
        BuildList(allGender.headCoverings_No_Hair, "HeadCoverings_No_Hair");
        BuildList(allGender.chest_Attachment, "All_03_Chest_Attachment");
        BuildList(allGender.back_Attachment, "All_04_Back_Attachment");
        BuildList(allGender.shoulder_Attachment_Right, "All_05_Shoulder_Attachment_Right");
        BuildList(allGender.shoulder_Attachment_Left, "All_06_Shoulder_Attachment_Left");
        BuildList(allGender.elbow_Attachment_Right, "All_07_Elbow_Attachment_Right");
        BuildList(allGender.elbow_Attachment_Left, "All_08_Elbow_Attachment_Left");
        BuildList(allGender.hips_Attachment, "All_09_Hips_Attachment");
        BuildList(allGender.knee_Attachement_Right, "All_10_Knee_Attachement_Right");
        BuildList(allGender.knee_Attachement_Left, "All_11_Knee_Attachement_Left");
        BuildList(allGender.elf_Ear, "Elf_Ear");
        
        
        
    }

    void BuildList(List<GameObject> targetList, string characterPart)
    {
        Transform[] rootTransform = gameObject.GetComponentsInChildren<Transform>();
        
        // declare target root transform
        Transform targetRoot = null;

        // find character parts parent object in the scene
        foreach (Transform t in rootTransform)
        {
            if (t.gameObject.name == characterPart)
            {
                targetRoot = t;
                break;
            }
        }

        // clears targeted list of all objects
        targetList.Clear();

        // cycle through all child objects of the parent object
        for (int i = 0; i < targetRoot.childCount; i++)
        {
            // get child gameobject index i
            GameObject go = targetRoot.GetChild(i).gameObject;

            // disable child object
            go.SetActive(false);

            // add object to the targeted object list
            targetList.Add(go);

        // collect the material for the character, only if null in the inspector;
            //if (!mat)
            //{
            //    if (go.GetComponent<SkinnedMeshRenderer>())
            //        mat = go.GetComponent<SkinnedMeshRenderer>().material;
            //}
        }
    }

    public void ActivateItem(GameObject go)
    {
        
        
        if (!enabledObjects.Contains(go))
        {
            for (int i = 0; i < enabledObjects.Count; i++)
            {
                int result = string.Compare(enabledObjects[i].name, 0, go.name, 0, go.name.Length - 2);
                
                if (result == 0)
                {
                    enabledObjects[i].SetActive(false);
                    enabledObjects.Remove(enabledObjects[i]);
                    break;
                }
            }

            enabledObjects.Add(go);
            go.SetActive(true);
        }
    }

    public void TempActivateItem(GameObject go)
    {
        if (go == null)
        {
            _tempObject.SetActive(false);
            
            foreach (GameObject g in enabledObjects)
            {
                g.SetActive(true);
            }

            return;
        }
        
        if (!enabledObjects.Contains(go))
        {
            for (int i = 0; i < enabledObjects.Count; i++)
            {
                int result = string.Compare(enabledObjects[i].name, 0, go.name, 0, go.name.Length - 2);

                if (result == 0)
                {
                    

                    enabledObjects[i].SetActive(false);
                    // enabledObjects.Remove(enabledObjects[i]);

                    foreach (Transform child in _allChildObjects)
                    {
                        if (child.name == go.name)
                        {
                            _tempObject = child.gameObject;
                            child.gameObject.SetActive(true);
                            //child.gameObject.SetActive(true);
                            //enabledObjects.Add(child.gameObject);
                            break;
                        }
                    }
                }
            }
        }
    }

    public void ChangeGender()
    {
        // disable any enabled objects before clear
        if (enabledObjects.Count != 0)
        {
            foreach (GameObject g in enabledObjects)
            {
                g.SetActive(false);
            }
        }

        enabledObjects.Clear();


        if (isMale)
        {
            ActivateItem(male.headAllElements[0]);
            ActivateItem(male.eyebrow[0]);
            ActivateItem(male.facialHair[0]);
            ActivateItem(male.torso[0]);
            ActivateItem(male.arm_Upper_Right[0]);
            ActivateItem(male.arm_Upper_Left[0]);
            ActivateItem(male.arm_Lower_Right[0]);
            ActivateItem(male.arm_Lower_Left[0]);
            ActivateItem(male.hand_Right[0]);
            ActivateItem(male.hand_Left[0]);
            ActivateItem(male.hips[0]);
            ActivateItem(male.leg_Right[0]);
            ActivateItem(male.leg_Left[0]);
            ActivateItem(allGender.all_Hair[0]);


        }
        else
        {
            ActivateItem(female.headAllElements[0]);
            ActivateItem(female.eyebrow[0]);
            ActivateItem(female.torso[0]);
            ActivateItem(female.arm_Upper_Right[0]);
            ActivateItem(female.arm_Upper_Left[0]);
            ActivateItem(female.arm_Lower_Right[0]);
            ActivateItem(female.arm_Lower_Left[0]);
            ActivateItem(female.hand_Right[0]);
            ActivateItem(female.hand_Left[0]);
            ActivateItem(female.hips[0]);
            ActivateItem(female.leg_Right[0]);
            ActivateItem(female.leg_Left[0]);
            ActivateItem(allGender.all_Hair[0]);

        }
    }

    public void ChangeMaterialColor(string target, Color color)
    {
        switch (target.ToString())
        {
            case "_Color_Skin":
                mat.SetColor("_Color_Skin", color);
                mat.SetColor("_Color_Stubble", color);
                break;
            case "_Color_Hair":
                mat.SetColor("_Color_Hair", color);
                break;
            case "_Color_Eyes":
                mat.SetColor("_Color_Eyes", color);
                break;
            case "_Color_BodyArt":
                mat.SetColor("_Color_BodyArt", color);
                break;

        }
    }    
   

    // classe for keeping the lists organized, allows for simple switching from male/female objects
    [System.Serializable]
    public class CharacterObjectGroups
    {
        public List<GameObject> headAllElements;
        public List<GameObject> headNoElements;
        public List<GameObject> eyebrow;
        public List<GameObject> facialHair;
        public List<GameObject> torso;
        public List<GameObject> arm_Upper_Right;
        public List<GameObject> arm_Upper_Left;
        public List<GameObject> arm_Lower_Right;
        public List<GameObject> arm_Lower_Left;
        public List<GameObject> hand_Right;
        public List<GameObject> hand_Left;
        public List<GameObject> hips;
        public List<GameObject> leg_Right;
        public List<GameObject> leg_Left;
    }
    
    

    // classe for keeping the lists organized, allows for organization of the all gender items
    [System.Serializable]
    public class CharacterObjectListsAllGender
    {
        public List<GameObject> headCoverings_Base_Hair;
        public List<GameObject> headCoverings_No_FacialHair;
        public List<GameObject> headCoverings_No_Hair;
        public List<GameObject> all_Hair;
        public List<GameObject> all_Head_Attachment;
        public List<GameObject> chest_Attachment;
        public List<GameObject> back_Attachment;
        public List<GameObject> shoulder_Attachment_Right;
        public List<GameObject> shoulder_Attachment_Left;
        public List<GameObject> elbow_Attachment_Right;
        public List<GameObject> elbow_Attachment_Left;
        public List<GameObject> hips_Attachment;
        public List<GameObject> knee_Attachement_Right;
        public List<GameObject> knee_Attachement_Left;
        public List<GameObject> all_12_Extra;
        public List<GameObject> elf_Ear;

        
    }

    public List<string> ColorTargets = new List<string>();
    
    public void SaveCustomization()
    {
        GameObject.Find("QuestCustomize").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(2, 0); // QUEST: Test Customization - Accept customization

        _player.GetComponent<PlayerController>().isCustomizing = false;
        
        UI_Inventory.GetComponent<Canvas>().enabled = false;

        Cursor.lockState = CursorLockMode.Locked;

        UI_Customize.SetActive(false);

        Disable();
    }
    


    public void Close(InputAction.CallbackContext context)
    {
        if (!gameObject.GetComponent<PlayerController>().isCustomizing) return;

        
        Cursor.lockState = CursorLockMode.Locked;


        if (_player == null) return;
        
        switch (context.phase)
        {
            case InputActionPhase.Started:

                GameObject.Find("QuestCustomize").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(0, 1); // QUEST: Test Customization - Open/Close customization

                UI_Inventory.GetComponent<Canvas>().enabled = false;
                UI_Customize.SetActive(false);

                Disable();
                break;
        }
    }

    public void MoveCamera(InputAction.CallbackContext context)
    {
        if (_player == null || !gameObject.GetComponent<PlayerController>().isCustomizing) return;
        
        switch (context.phase)
        {
            case InputActionPhase.Started:
                Cursor.lockState = CursorLockMode.Confined;
                break;
            case InputActionPhase.Canceled:
                Cursor.lockState = CursorLockMode.None;
                break;
        }
    }
    
    

    void Disable()
    {
        _player.GetComponent<PlayerController>().enabled = true;
        _player.GetComponent<PlayerAiming>().enabled = true;


        _allChildObjectsIndex = new int[enabledObjects.Count];

        for (int i = 0; i < enabledObjects.Count; i++)
        {
            for (int j = 0; j < _allChildObjects.Length; j++)
            {
                if (_allChildObjects[j].name == enabledObjects[i].name)
                {
                    _allChildObjectsIndex[i] = j;
                }
            }
        }

        if (IsHost)
        {
            UpdateCharacterServerRpc(_allChildObjectsIndex);
        }
        else
        {
            UpdateCharacterClientRpc(_allChildObjectsIndex);
        }

        _player.GetComponent<PlayerController>().isCustomizing = false;

        enabled = false;
    }
}


