using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using PsychoticLab;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

public class CustomizeUI : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button _maleButton;
    [SerializeField] private Button _femaleButton;
    [SerializeField] private Button _headAllElements;
    [SerializeField] private Button _allHair;
    [SerializeField] private Button _eyebrow;
    [SerializeField] private Button _facialHair;
    [SerializeField] private Button _accept;

    [Header("Ui Buttons Prefabs")]
    [SerializeField] private Button _buttonPrefab;
    [SerializeField] private Button _colorButtonPrefab;

    [Header("UI Canvas")]
    [SerializeField] private Canvas _options;
    [SerializeField] private Canvas _skinColors;
    [SerializeField] private Canvas _hairColors;
    [SerializeField] private Canvas _eyeColors;
    [SerializeField] private Canvas _bodyArtColors;

    [Header("UI Lists")]
    public CharacterObjectGroups male;
    public CharacterObjectGroups female;
    public CharacterObjectListsAllGender allGender;

    [Header("Player")]
    public GameObject _player;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private CustomizePlayerData _customizePlayerData;
    [SerializeField] private PlayerAiming _playerAiming;
    [SerializeField] private ShowInventory _showInventory;
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private PlayerControls _playerControls;
    [SerializeField] private CinemachineVirtualCamera _vcam;
    [SerializeField] private GameObject _tempObject = null;

    [Header("Children/Objects")]
    [SerializeField] private ScriptableObject _allPlayerModules;

    [SerializeField] private GameObject _UI_Customize;
    [SerializeField] private GameObject _UI_Inventory;

    [SerializeField] private Transform _lookAt;

    

    public Vector3 cameraPositionWhenCustomizing;
    public Vector3 playerPositionWhenCustomizing;
    public float playerRotationWhenCustomizing;


    public int[] activePartsIndex;

    public List<GameObject> enabledObjects = new List<GameObject>();



    public List<GameObject> playerList = new List<GameObject>();


    [SerializeField] private bool _isMale = true;

    //[SerializeField] private TMP_Dropdown headDropdown;

    [Header("Material")]
    [SerializeField] private Material _mat;

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
    public Color[] colorHair = { new Color(0.3098039f, 0.254902f, 0.1764706f), new Color(0.2196079f, 0.2196079f, 0.2196079f), new Color(0.8313726f, 0.6235294f, 0.3607843f), new Color(0.8901961f, 0.7803922f, 0.5490196f), new Color(0.8000001f, 0.8196079f, 0.8078432f), new Color(0.6862745f, 0.4f, 0.2352941f), new Color(0.5450981f, 0.427451f, 0.2156863f), new Color(0.3098039f, 0.254902f, 0.1764706f), new Color(0.1764706f, 0.1686275f, 0.1686275f), new Color(0.3843138f, 0.2352941f, 0.0509804f), new Color(0.6196079f, 0.6196079f, 0.6196079f), new Color(0.6196079f, 0.6196079f, 0.6196079f), new Color(0.2431373f, 0.2039216f, 0.145098f), new Color(0.1764706f, 0.1686275f, 0.1686275f) };

    [Header("Eye Colors")]
    public Color[] colorEyes = { new Color(1.00000f, 0.87843f, 0.74118f), new Color(1.00000f, 0.87843f, 0.74118f), new Color(1.00000f, 0.87843f, 0.74118f), new Color(1.00000f, 0.87843f, 0.74118f), new Color(1.00000f, 0.87843f, 0.74118f), new Color(1.00000f, 0.87843f, 0.74118f), new Color(1.00000f, 0.87843f, 0.74118f), new Color(1.00000f, 0.87843f, 0.74118f), };

    [Header("Scar Colors")]
    public Color[] colorScar = { };

    [Header("Body Art Colors")]
    public Color[] colorBodyArt = { new Color(0.0509804f, 0.6745098f, 0.9843138f), new Color(0.7215686f, 0.2666667f, 0.2666667f), new Color(0.3058824f, 0.7215686f, 0.6862745f), new Color(0.9254903f, 0.882353f, 0.8509805f), new Color(0.3098039f, 0.7058824f, 0.3137255f), new Color(0.5294118f, 0.3098039f, 0.6470588f), new Color(0.8666667f, 0.7764707f, 0.254902f), new Color(0.2392157f, 0.4588236f, 0.8156863f) };


    private void Awake()
    {
        _UI_Inventory = GameObject.FindGameObjectWithTag("UI_Inventory");
        _UI_Customize = GameObject.FindGameObjectWithTag("UI_Customize");
    }

    private bool _startUpdate = false;

    private void Update()
    {
        if (!_startUpdate) return;

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

    private Quaternion _oldCameraRotation;

    public void Initialize()
    {
        DestroyColorChildren();

        _playerController = _player.GetComponent<PlayerController>();
        _playerAiming = _player.GetComponent<PlayerAiming>();
        _customizePlayerData = _player.GetComponent<CustomizePlayerData>();
        GetComponent<Canvas>().enabled = true;


        _showInventory = _player.GetComponent<ShowInventory>();
        _playerControls = new PlayerControls();
        _playerAnimator = _player.GetComponent<Animator>();

        _maleButton.onClick.AddListener(() => { OnClickMaleButton(); });

        _femaleButton.onClick.AddListener(() => { OnClickFemaleButton(); });

        _headAllElements.onClick.AddListener(() => { OnClickHeadAllElements(); });

        _allHair.onClick.AddListener(() => { OnClickAll_Hair(); });

        _eyebrow.onClick.AddListener(() => { OnClickEyebrow(); });

        _facialHair.onClick.AddListener(() => { OnClickFacialHair(); });

        _accept.onClick.AddListener(() => { OnClickAccept(); });


        SkincolorShow();
        HaircolorShow();
        EyecolorShow();
        BodyArtcolorShow();

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

        //SetChildIndexList();
        


        _playerControls.Enable();

        _playerControls.Customize.Close.started += Close;
        // _playerControls.Customize.Close.canceled += Close;

        _playerControls.Customize.MoveCamera.started += MoveCamera;
        _playerControls.Customize.MoveCamera.canceled += MoveCamera;

        _vcam = _player.GetComponent<PlayerController>().GetCamera().GetComponent<CinemachineVirtualCamera>();

        // change _vcam body shoulder offset
        _vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>().ShoulderOffset = new Vector3(0f, -1.5f, 0f);
        
        //enabled = false;

        //FindChildObjects(_player.transform);
        

        _player.GetComponent<PlayerController>().isCustomizing = true;

        _startUpdate = true;
        //gameObject.SetActive(false);

        activePartsIndex = new int[enabledObjects.Count];

        for (int i = 0; i < enabledObjects.Count; i++)
        {
            activePartsIndex[i] = _customizePlayerData.allChildObjects.IndexOf(enabledObjects[i]);
        }

        _customizePlayerData.UpdateCharacterServerRpc(activePartsIndex);
    }

    

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
        Transform[] rootTransform = _player.transform.Find("Modular_Characters").GetComponentsInChildren<Transform>();
        //Transform[] rootTransform = _player.GetComponentsInChildren<Transform>();

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
            if (!_mat)
            {
                if (go.GetComponent<SkinnedMeshRenderer>())
                    _mat = go.GetComponent<SkinnedMeshRenderer>().material;
            }
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

                    foreach (GameObject child in _customizePlayerData.allChildObjects)
                    {

                        if (child.name == go.name)
                        {
                            _tempObject = child;
                            
                            child.SetActive(true);
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


        if (_isMale)
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
        Debug.Log(target + " " + color);
        
        switch (target.ToString())
        {
            case "_Color_Skin":
                _mat.SetColor("_Color_Skin", color);
                _mat.SetColor("_Color_Stubble", color);
                break;
            case "_Color_Hair":
                _mat.SetColor("_Color_Hair", color);
                break;
            case "_Color_Eyes":
                _mat.SetColor("_Color_Eyes", color);
                break;
            case "_Color_BodyArt":
                _mat.SetColor("_Color_BodyArt", color);
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


        Cursor.lockState = CursorLockMode.Locked;

        _UI_Inventory.GetComponent<Canvas>().enabled = false;
        _UI_Customize.GetComponent<Canvas>().enabled = false;
        //_UI_Customize.SetActive(false);

        activePartsIndex = new int[enabledObjects.Count];

        for (int i = 0; i < enabledObjects.Count; i++)
        {
            activePartsIndex[i] = _customizePlayerData.allChildObjects.IndexOf(enabledObjects[i]);
        }

        _customizePlayerData.UpdateCharacterServerRpc(activePartsIndex);

        _player.GetComponent<PlayerController>().enabled = true;
        _player.GetComponent<PlayerAiming>().enabled = true;

        _vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>().ShoulderOffset = new Vector3(.5f, -.5f, 0f);

        _player.GetComponent<PlayerController>().isCustomizing = false;
    }



    public void Close(InputAction.CallbackContext context)
    {
        if (!_player.GetComponent<PlayerController>().isCustomizing) return;


        //Cursor.lockState = CursorLockMode.Locked;


        if (_player == null) return;

        switch (context.phase)
        {
            case InputActionPhase.Started:

                //GameObject.Find("QuestCustomize").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(0, 1); // QUEST: Test Customization - Open/Close customization

                //_UI_Inventory.GetComponent<Canvas>().enabled = false;
                //_UI_Customize.GetComponent<Canvas>().enabled = false;
                //_UI_Customize.SetActive(false);

                //Disable();
                break;
        }
    }

    public void MoveCamera(InputAction.CallbackContext context)
    {
        if (_player == null || !_player.GetComponent<PlayerController>().isCustomizing) return;

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

    void OnClickMaleButton()
    {
        if (_isMale)
            return;
        
        GameObject.Find("QuestCustomize").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(1, 1); // QUEST: Test Customization - Open/Close customization

        //facialHair.gameObject.SetActive(true);
        _facialHair.transform.GetChild(0).gameObject.SetActive(true);
        _facialHair.GetComponent<Image>().enabled = true;
        _facialHair.GetComponent<Button>().interactable = true;
        _isMale = true;
        _isMale = true;
        DestroyChildren();
        ChangeGender();

    }

    void OnClickFemaleButton()
    {
        if (!_isMale)
            return;
        GameObject.Find("QuestCustomize").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(1, 0); // QUEST: Test Customization - Open/Close customization
        //facialHair.gameObject.SetActive(false);
        _facialHair.transform.GetChild(0).gameObject.SetActive(false);
        _facialHair.GetComponent<Image>().enabled = false;
        _facialHair.GetComponent<Button>().interactable = false;
        _isMale = false;
        _isMale = false;
        DestroyChildren();
        ChangeGender();
    }

    void OnClickHeadAllElements()
    {
        int counter = 1;

        DestroyChildren();

        if (_isMale)
        {

            foreach (GameObject element in male.headAllElements)
            {
                Button button = Instantiate(_buttonPrefab, _options.transform);
                button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
                button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
                button.onClick.AddListener(() => { OnClickHeadElement(element); });
                counter++;
            }
        }
        else
        {
            foreach (GameObject element in female.headAllElements)
            {
                Button button = Instantiate(_buttonPrefab, _options.transform);
                button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
                button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
                button.onClick.AddListener(() => { OnClickHeadElement(element); });
                counter++;
            }
        }

        SetMouseoverOptions();
    }

    void OnClickHeadElement(GameObject element)
    {
        Debug.Log(element.name);
        ActivateItem(element);
    }

    void OnClickAll_Hair()
    {
        int counter = 1;

        DestroyChildren();

        foreach (GameObject element in allGender.all_Hair)
        {
            Button button = Instantiate(_buttonPrefab, _options.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
            button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
            button.onClick.AddListener(() => { OnClickHairElement(element); });
            counter++;
        }

        SetMouseoverOptions();
    }


    void OnClickHairElement(GameObject element)
    {
        ActivateItem(element);
    }

    void OnClickEyebrow()
    {
        int counter = 1;

        DestroyChildren();

        if (_isMale)
        {
            foreach (GameObject element in male.eyebrow)
            {
                Button button = Instantiate(_buttonPrefab, _options.transform);
                button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
                button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
                button.onClick.AddListener(() => { OnClickEyebrowElement(element); });
                counter++;
            }
        }
        else
        {
            foreach (GameObject element in female.eyebrow)
            {
                Button button = Instantiate(_buttonPrefab, _options.transform);
                button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
                button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
                button.onClick.AddListener(() => { OnClickEyebrowElement(element); });
                counter++;
            }
        }

        SetMouseoverOptions();
    }

    void OnClickEyebrowElement(GameObject element)
    {
        Debug.Log(element.name);
        ActivateItem(element);
    }


    void OnClickFacialHair()
    {
        if (!_isMale)
            return;

        int counter = 1;

        DestroyChildren();

        foreach (GameObject element in male.facialHair)
        {
            Button button = Instantiate(_buttonPrefab, _options.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
            button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
            button.onClick.AddListener(() => { OnClickFacialHairElement(element); });
            counter++;
        }
        
        SetMouseoverOptions();
    }

    void OnClickFacialHairElement(GameObject element)
    {
        ActivateItem(element);
    }

    void OnClickAccept()
    {
        SaveCustomization();
    }

    void OnClickElf_Ear(bool value)
    {
        int counter = 1;

        DestroyChildren();

        foreach (GameObject element in allGender.elf_Ear)
        {
            Button button = Instantiate(_buttonPrefab, _options.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
            button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
            button.onClick.AddListener(() => { OnClickElf_EarElement(element); });
            counter++;
        }

        SetMouseoverOptions();
    }

    void OnClickElf_EarElement(GameObject element)
    {
        Debug.Log(element.name);
        ActivateItem(element);
    }

    void SkincolorShow()
    {
        foreach (Color color in colorSkin)
        {
            Button button = Instantiate(_colorButtonPrefab, _skinColors.transform);
            button.GetComponent<Image>().color = color;
            button.onClick.AddListener(() => { ChangeMaterialColor("_Color_Skin", color); });
        }
    }

    void HaircolorShow()
    {
    
        foreach (Color color in colorHair)
        {
            Button button = Instantiate(_colorButtonPrefab, _hairColors.transform);
            button.GetComponent<Image>().color = color;
            button.onClick.AddListener(() => { ChangeMaterialColor("_Color_Hair", color); });
        }
    }

    void EyecolorShow()
    {
        foreach (Color color in colorEyes)
        {
            Button button = Instantiate(_colorButtonPrefab, _eyeColors.transform);
            button.GetComponent<Image>().color = color;
            button.onClick.AddListener(() => { ChangeMaterialColor("_Color_Eyes", color); });
        }
    }

    void BodyArtcolorShow()
    {
        foreach (Color color in colorBodyArt)
        {
            Button button = Instantiate(_colorButtonPrefab, _bodyArtColors.transform);
            button.GetComponent<Image>().color = color;
            button.onClick.AddListener(() => { ChangeMaterialColor("_Color_BodyArt", color); });
        }
    }



    void DestroyChildren()
    {
        for (int i = _options.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = _options.transform.GetChild(i);
            // code to remove the child
            Destroy(child.gameObject);
        }

        
    }

    void DestroyColorChildren()
    {
        for (int i = _skinColors.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = _skinColors.transform.GetChild(i);
            // code to remove the child
            Destroy(child.gameObject);
        }

        for (int i = _hairColors.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = _hairColors.transform.GetChild(i);
            // code to remove the child
            Destroy(child.gameObject);
        }

        for (int i = _eyeColors.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = _eyeColors.transform.GetChild(i);
            // code to remove the child
            Destroy(child.gameObject);
        }

        for (int i = _bodyArtColors.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = _bodyArtColors.transform.GetChild(i);
            // code to remove the child
            Destroy(child.gameObject);
        }
    }

    void SetMouseoverOptions()
    {
        for (int i = 0; i < _options.transform.childCount; i++)
        {
            _options.transform.GetChild(i).GetComponent<MouseoverOption>().customizeUI = this;
        }
    }
    

}
