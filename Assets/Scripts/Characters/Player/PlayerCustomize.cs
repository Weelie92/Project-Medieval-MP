using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using PsychoticLab;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using static Cinemachine.AxisState;
using Cursor = UnityEngine.Cursor;

public class PlayerCustomize : MonoBehaviour
{
    private GameObject _player;
    private ShowInventory _showInventory;
    private Animator _playerAnimator;
    private PlayerControls _playerControls;
    private CinemachineVirtualCamera _vcam;
    private Cinemachine3rdPersonFollow _vcamFollow;
    private PlayerAiming _playerAiming; 
    public GameObject _tempObject = null;
    private Transform[] _allChildObjects;

    public Canvas customizeCanvas;
    public Canvas inventoryToolbar;

    public Transform _lookAt;
    [SerializeField] private Cinemachine.AxisState xAxis;
    [SerializeField] private Cinemachine.AxisState yAxis;
    private Cinemachine.CinemachineInputProvider inputAxisProvider;

    public bool isMale = true;

    private Quaternion _targetRotation;
    private bool _cameraMoving = false;
    [HideInInspector] public bool _movingCamera = false;

    [Header("Material")]
    public Material mat;

    [Header("Gear Colors")]
    public Color[] colorGearPrimary = { new Color(0.2862745f, 0.4f, 0.4941177f), new Color(0.4392157f, 0.1960784f, 0.172549f), new Color(0.3529412f, 0.3803922f, 0.2705882f), new Color(0.682353f, 0.4392157f, 0.2196079f), new Color(0.4313726f, 0.2313726f, 0.2705882f), new Color(0.5921569f, 0.4941177f, 0.2588235f), new Color(0.482353f, 0.4156863f, 0.3529412f), new Color(0.2352941f, 0.2352941f, 0.2352941f), new Color(0.2313726f, 0.4313726f, 0.4156863f) };
    public Color[] colorGearSecondary = { new Color(0.7019608f, 0.6235294f, 0.4666667f), new Color(0.7372549f, 0.7372549f, 0.7372549f), new Color(0.1647059f, 0.1647059f, 0.1647059f), new Color(0.2392157f, 0.2509804f, 0.1882353f) };

    [Header("Metal Colors")]
    public Color[] colorMetalPrimary = { new Color(0.6705883f, 0.6705883f, 0.6705883f), new Color(0.5568628f, 0.5960785f, 0.6392157f), new Color(0.5568628f, 0.6235294f, 0.6f), new Color(0.6313726f, 0.6196079f, 0.5568628f), new Color(0.6980392f, 0.6509804f, 0.6196079f) };
    public Color[] colorMetalSeconday = { new Color(0.3921569f, 0.4039216f, 0.4117647f), new Color(0.4784314f, 0.5176471f, 0.5450981f), new Color(0.3764706f, 0.3607843f, 0.3372549f), new Color(0.3254902f, 0.3764706f, 0.3372549f), new Color(0.4f, 0.4039216f, 0.3568628f) };

    [Header("Leather Colors")]
    public Color[] colorLeatherPrimary;
    public Color[] colorLeatherSecondary;

    [Header("Skin Colors")]
    public Color[] colorSkin = { new Color(1.00000f, 0.87843f, 0.74118f), new Color(1f, 0.8000001f, 0.682353f), new Color(1f, 0.80392f, 0.58039f), new Color(0.87843f, 0.68235f, 0.41176f), new Color(0.8196079f, 0.6352941f, 0.4588236f), new Color(0.5647059f, 0.4078432f, 0.3137255f), new Color(0.55294f, 0.33333f, 0.14118f), new Color(0.43529f, 0.30980f, 0.11373f) };

    [Header("Hair Colors")]
    public Color[] colorHair = { new Color(0.3098039f, 0.254902f, 0.1764706f), new Color(0.2196079f, 0.2196079f, 0.2196079f), new Color(0.8313726f, 0.6235294f, 0.3607843f), new Color(0.8901961f, 0.7803922f, 0.5490196f), new Color(0.8000001f, 0.8196079f, 0.8078432f), new Color(0.6862745f, 0.4f, 0.2352941f), new Color(0.5450981f, 0.427451f, 0.2156863f), new Color(0.3098039f, 0.254902f, 0.1764706f), new Color(0.1764706f, 0.1686275f, 0.1686275f), new Color(0.3843138f, 0.2352941f, 0.0509804f), new Color(0.6196079f, 0.6196079f, 0.6196079f), new Color(0.6196079f, 0.6196079f, 0.6196079f), new Color(0.2431373f, 0.2039216f, 0.145098f), new Color(0.1764706f, 0.1686275f, 0.1686275f)};

    [Header("Eye Colors")]
    public Color[] colorEyes = { new Color(1.00000f, 0.87843f, 0.74118f), new Color(1.00000f, 0.87843f, 0.74118f), new Color(1.00000f, 0.87843f, 0.74118f), new Color(1.00000f, 0.87843f, 0.74118f), new Color(1.00000f, 0.87843f, 0.74118f), new Color(1.00000f, 0.87843f, 0.74118f), new Color(1.00000f, 0.87843f, 0.74118f), new Color(1.00000f, 0.87843f, 0.74118f), };

    [Header("Scar Colors")]
    public Color[] colorScar = { };

    [Header("Body Art Colors")]
    public Color[] colorBodyArt = { new Color(0.0509804f, 0.6745098f, 0.9843138f), new Color(0.7215686f, 0.2666667f, 0.2666667f), new Color(0.3058824f, 0.7215686f, 0.6862745f), new Color(0.9254903f, 0.882353f, 0.8509805f), new Color(0.3098039f, 0.7058824f, 0.3137255f), new Color(0.5294118f, 0.3098039f, 0.6470588f), new Color(0.8666667f, 0.7764707f, 0.254902f), new Color(0.2392157f, 0.4588236f, 0.8156863f) };



    void Awake()
    {
        _player = GameObject.Find("Player");
        _showInventory = gameObject.GetComponent<ShowInventory>();
        _playerControls = new PlayerControls();
        _playerAiming = GetComponent<PlayerAiming>();
        _playerAnimator = GetComponent<Animator>();
        _allChildObjects = _player.GetComponentsInChildren<Transform>();

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

        _vcam = GetComponentInChildren<CinemachineVirtualCamera>();

        inputAxisProvider = GetComponent<Cinemachine.CinemachineInputProvider>();
        xAxis.SetInputAxisProvider(0, inputAxisProvider);
        yAxis.SetInputAxisProvider(1, inputAxisProvider);
    }


    public void Initialize(Vector3 position)
    {
        if (_showInventory.showInventory)
        {
            gameObject.GetComponent<PlayerCustomize>().enabled = false;
            return;
        }

        inventoryToolbar.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;



        _vcamFollow = _vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        _player.GetComponent<PlayerController>()._isCustomizing = true;
        _player.GetComponent<PlayerController>().enabled = false;
        _player.GetComponent<HeadLookAtDirection>().enabled = false;
        _player.GetComponent<PlayerAiming>().enabled = false;
        _player.GetComponent<CheckInteraction>().enabled = false;
        
        _player.GetComponent<ShowInventory>().toolbar.SetActive(false);


        _playerAnimator.SetBool("isMoving", false);
        _playerAnimator.SetBool("isRunning", false);
        _playerAnimator.SetBool("isOffhanding", false);
        _playerAnimator.SetBool("isFalling", false);



        xAxis.Value = 0;
        yAxis.Value = 0;

        _targetRotation = Quaternion.Euler(position);
        

        _cameraMoving = true;

        customizeCanvas.gameObject.SetActive(true);
        

        Invoke(nameof(CameraStop), 1f);

        //_targetRotation = Quaternion.LookRotation(-Camera.main.transform.forward);
        //_currentRotation = _targetRotation.eulerAngles.y;


    }
    
    private void Update()
    {
        if (_player.GetComponent<PlayerController>()._isCustomizing)
        {
            if (_cameraMoving)
            {
                _vcamFollow.CameraDistance = Mathf.Lerp(_vcamFollow.CameraDistance, 1.7f, Time.deltaTime * 5f);
                _vcamFollow.ShoulderOffset = Vector3.Lerp(_vcamFollow.ShoulderOffset, new Vector3(0, -1, 0), Time.deltaTime * 5f);
                
                _lookAt.rotation = Quaternion.Slerp(_lookAt.rotation, _targetRotation, 5f * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-Camera.main.transform.forward, Vector3.up), Time.deltaTime * 5f);
            }
            else if (_movingCamera)
            {
                xAxis.Update(Time.deltaTime);
                yAxis.Update(Time.deltaTime);
                _lookAt.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);
            }
            
            
        }
        else
        {            
            _vcamFollow.CameraDistance = Mathf.Lerp(_vcamFollow.CameraDistance, 3f, Time.deltaTime * 5f);
            _vcamFollow.ShoulderOffset = Vector3.Lerp(_vcamFollow.ShoulderOffset, new Vector3(1, -.5f, 0), Time.deltaTime * 5f);

            _lookAt.rotation = Quaternion.Slerp(_lookAt.rotation, _targetRotation, 5f * Time.deltaTime);
        }
    }


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
                    Debug.Log("Changed");
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
    

    void CameraStop()
    {
        _cameraMoving = false;
    }

    public void SaveCustomization()
    {
        GameObject.Find("Quest").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(2, 0); // QUEST
        _player.GetComponent<PlayerController>()._isCustomizing = false;
        _player.GetComponent<ShowInventory>().toolbar.SetActive(true);
        inventoryToolbar.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;

        customizeCanvas.gameObject.SetActive(false);

        Invoke(nameof(Disable), 1f);
    }

    





    public void Close(InputAction.CallbackContext context)
    {
        if (!gameObject.GetComponent<PlayerController>()._isCustomizing) return;

        
        Cursor.lockState = CursorLockMode.Locked;

        GameObject.Find("Quest").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(0, 0); // QUEST

        if (_player == null) return;
        
        switch (context.phase)
        {
            case InputActionPhase.Started:
                
                _player.GetComponent<PlayerController>()._isCustomizing = false;
                _player.GetComponent<ShowInventory>().toolbar.SetActive(true);
                
                inventoryToolbar.gameObject.SetActive(true);
                customizeCanvas.gameObject.SetActive(false);

                Invoke(nameof(Disable), 1f);
                break;
        }
    }

    public void MoveCamera(InputAction.CallbackContext context)
    {
        if (_player == null || !gameObject.GetComponent<PlayerController>()._isCustomizing) return;
        
        switch (context.phase)
        {
            case InputActionPhase.Started:
                _movingCamera = true;
                Cursor.lockState = CursorLockMode.Confined;
                break;
            case InputActionPhase.Canceled:
                Cursor.lockState = CursorLockMode.None;
                _movingCamera = false;
                break;
        }
    }
    
    void Disable()
    {
        _player.GetComponent<PlayerController>().enabled = true;
        _player.GetComponent<HeadLookAtDirection>().enabled = true;
        _player.GetComponent<PlayerAiming>().enabled = true;
        _player.GetComponent<CheckInteraction>().enabled = true;


        customizeCanvas.gameObject.SetActive(false);


        _player.GetComponent<PlayerAiming>().xAxis.Value = 0;
        _player.GetComponent<PlayerAiming>().yAxis.Value = 0;

        enabled = false;
    }
}


