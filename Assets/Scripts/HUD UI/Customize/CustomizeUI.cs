using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private PlayerControls _playerControls;
    [SerializeField] private CinemachineVirtualCamera _vcam;
    [SerializeField] private GameObject _tempObject = null;

    [Header("Children/Objects")]
    [SerializeField] private ScriptableObject _allPlayerModules;

    [SerializeField] private Transform _lookAt;

    public Vector3 cameraPositionWhenCustomizing;
    public Vector3 playerPositionWhenCustomizing;
    public float playerRotationWhenCustomizing;

    public int[] activePartsIndex;

    private List<GameObject> _oldEnabledObjects = new List<GameObject>();
    public List<GameObject> enabledObjects = new List<GameObject>();

    public List<GameObject> playerList = new List<GameObject>();


    [SerializeField] private bool _isMale = true;

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

    [SerializeField] private bool _isPlayerMoving = false;
    [SerializeField] private bool _isCameraMoving = false;

    [SerializeField] private Vector3 targetShoulderOffset = Vector3.zero;

    

    private void Update()
    {
        if (!_playerController) return;
        
        if (_playerController.isCustomizing)
        {
            targetShoulderOffset = new Vector3(0, -1f, 0);
        }
        else if (_isCameraMoving)
        {
            targetShoulderOffset = new Vector3(.5f, -.5f, 0);
        }

        UpdateCameraPosition();

        if (!_isPlayerMoving) return;
        
        MoveAndRotatePlayer();
        PreventCameraRotation();
    }

    private void UpdateCameraPosition()
    {
        _vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>().ShoulderOffset = Vector3.Lerp(_vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>().ShoulderOffset, targetShoulderOffset, 1f * Time.deltaTime);
        
        if (Vector3.Distance(_vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>().ShoulderOffset, targetShoulderOffset) < 0.1f)
        {
            _isCameraMoving = false;
        }
    }

    private void MoveAndRotatePlayer()
    {
        float distance = Vector3.Distance(_player.transform.position, playerPositionWhenCustomizing);

        if (distance > 0.01f)
        {
            MoveTowardsTarget();
        }
        else
        {
            RotateTowardsTarget();
            if (Quaternion.Angle(_player.transform.rotation, Quaternion.Euler(0, playerRotationWhenCustomizing, 0)) < 1f)
            {
                _isPlayerMoving = false;
            }
        }
    }

    private void MoveTowardsTarget()
    {
        _player.transform.position = Vector3.MoveTowards(_player.transform.position, playerPositionWhenCustomizing, Time.deltaTime * 2f);
        _player.transform.rotation = Quaternion.RotateTowards(_player.transform.rotation, Quaternion.LookRotation(playerPositionWhenCustomizing - _player.transform.position, Vector3.up), Time.deltaTime * 360f);
        _playerAnimator.SetBool("isMoving", true);
    }

    private void RotateTowardsTarget()
    {
        _player.transform.rotation = Quaternion.RotateTowards(_player.transform.rotation, Quaternion.Euler(0, playerRotationWhenCustomizing, 0), Time.deltaTime * 180f);
        _playerAnimator.SetBool("isMoving", false);
    }

    private void PreventCameraRotation()
    {
        _playerAiming.cameraLookAt.eulerAngles = new Vector3(_playerAiming.yAxis.Value, _playerAiming.xAxis.Value, 0);
    }

    private bool _isFirstInitialization = true;
    
    public void Initialize()
    {
        if (_isFirstInitialization)
        {
            DestroyColorChildren();
            BuildActiveIndexArray();
            AssignPlayerComponents();
            AssignButtonListeners();
            ShowColorButtons();
            BuildLists();
            DisableEnabledObjects();

            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                _isMale = true;
                SetDefaultMaleCharacter();
            }
            else
            {
                _isMale = false;
                SetDefaultFemaleCharacter();
            }
            
            SetActiveParts();
            ActivateEnabledObjects();
            UpdateCharacterOnServer();
        }
        else
        {
            _playerController.isCustomizing = true;
            _isPlayerMoving = true;
            Cursor.lockState = CursorLockMode.None;

            SetOldEnabledObjects();
        }

        _isCameraMoving = true;

        EnablePlayerControls();
        EnableCanvas();

    }

    private void DestroyColorChildren()
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

    private void BuildActiveIndexArray()
    {
        activePartsIndex = new int[26];

        for (int i = 0; i < activePartsIndex.Length; i++)
        {
            activePartsIndex[i] = -1;
        }
    }
    
    private void AssignPlayerComponents()
    {
        //_player gets set when the player spawns
        _playerController = _player.GetComponent<PlayerController>();
        _playerAiming = _player.GetComponent<PlayerAiming>();
        _customizePlayerData = _player.GetComponent<CustomizePlayerData>();
        _playerAnimator = _player.GetComponent<Animator>();
        _playerControls = new PlayerControls();
        _vcam = _playerController.GetCamera().GetComponent<CinemachineVirtualCamera>();

        _playerController.isCustomizing = true;
    }

    private void EnableCanvas()
    {
        GetComponent<Canvas>().enabled = true;
    }

    private void AssignButtonListeners()
    {
        _maleButton.onClick.AddListener(() => { OnClickMaleButton(); });
        _femaleButton.onClick.AddListener(() => { OnClickFemaleButton(); });
        _headAllElements.onClick.AddListener(() => { OnClickHeadAllElements(); });
        _allHair.onClick.AddListener(() => { OnClickAll_Hair(); });
        _eyebrow.onClick.AddListener(() => { OnClickEyebrow(); });
        _facialHair.onClick.AddListener(() => { OnClickFacialHair(); });
        _accept.onClick.AddListener(() => { SaveCustomization(); });
    }

    private void ShowColorButtons()
    {
        SkincolorShow();
        HaircolorShow();
        EyecolorShow();
        BodyArtcolorShow();
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

    private void DisableEnabledObjects()
    {
        if (enabledObjects.Count != 0)
        {
            foreach (GameObject g in enabledObjects)
            {
                g.SetActive(false);
            }
            
            enabledObjects.Clear();
        }
    }

    private void SetDefaultMaleCharacter()
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
    
    private void SetDefaultFemaleCharacter()
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

    private void EnablePlayerControls()
    {
        _playerControls.Enable();

        if (!_isFirstInitialization) return;
        
        _playerControls.Customize.Close.started += Close;

        _playerControls.Customize.MoveCamera.started += MoveCamera;
        _playerControls.Customize.MoveCamera.canceled += MoveCamera;
    }

    private void SetOldEnabledObjects()
    {

        _oldEnabledObjects.Clear();

        foreach (GameObject obj in enabledObjects)
        {
            _oldEnabledObjects.Add(obj);
        }
    }

    private void UpdateCharacterOnServer()
    {
        _customizePlayerData.UpdateCharacterServerRpc(activePartsIndex);
    }

    private void BuildList(List<GameObject> targetList, string characterPart)
    {
        Transform[] rootTransforms = _player.transform.Find("Modular_Characters").GetComponentsInChildren<Transform>();

        Transform targetRoot = rootTransforms.FirstOrDefault(t => t.gameObject.name == characterPart);

        if (targetRoot == null)
        {
            return;
        }

        targetList.Clear();

        foreach (Transform childTransform in targetRoot)
        {
            GameObject child = childTransform.gameObject;
            child.SetActive(false);
            targetList.Add(child);

            if (!_mat && child.GetComponent<SkinnedMeshRenderer>())
            {
                _mat = child.GetComponent<SkinnedMeshRenderer>().material;
            }
        }
    }

    public void ActivateItem(GameObject go)
    {
        if (!enabledObjects.Contains(go))
        {
            DeactivateSimilarItems(go);
            enabledObjects.Add(go);
            go.SetActive(true);
        }
    }

    public void ActivateTempItem(GameObject go)
    {
        if (go == null)
        {
            DeactivateTempObject();
            ActivateEnabledObjects();
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

    private void DeactivateSimilarItems(GameObject go)
    {
        int index = FindSimilarItemIndex(go);

        if (index != -1)
        {
            GameObject similarItem = enabledObjects[index];
            similarItem.SetActive(false);
            enabledObjects.Remove(similarItem);
        }
    }

    private int FindSimilarItemIndex(GameObject go)
    {
        for (int i = 0; i < enabledObjects.Count; i++)
        {
            int result = string.Compare(enabledObjects[i].name, 0, go.name, 0, go.name.Length - 2);

            if (result == 0)
            {
                return i;
            }
        }

        return -1;
    }

    private void DeactivateTempObject()
    {
        if (_tempObject != null)
        {
            _tempObject.SetActive(false);
            _tempObject = null;
        }
    }

    private void ActivateEnabledObjects()
    {
        foreach (GameObject g in enabledObjects)
        {
            g.SetActive(true);
        }
    }

    public void ChangeGender()
    {
        List<GameObject> itemsToActivate;

        if (_isMale)
        {
            itemsToActivate = new List<GameObject>
            {
                male.headAllElements[0],
                male.eyebrow[0],
                male.facialHair[0],
                male.torso[0],
                male.arm_Upper_Right[0],
                male.arm_Upper_Left[0],
                male.arm_Lower_Right[0],
                male.arm_Lower_Left[0],
                male.hand_Right[0],
                male.hand_Left[0],
                male.hips[0],
                male.leg_Right[0],
                male.leg_Left[0],
                allGender.all_Hair[0]
            };
        }
        else
        {
            itemsToActivate = new List<GameObject>
            {
                female.headAllElements[0],
                female.eyebrow[0],
                female.torso[0],
                female.arm_Upper_Right[0],
                female.arm_Upper_Left[0],
                female.arm_Lower_Right[0],
                female.arm_Lower_Left[0],
                female.hand_Right[0],
                female.hand_Left[0],
                female.hips[0],
                female.leg_Right[0],
                female.leg_Left[0],
                allGender.all_Hair[0]
            };
        }

        DeactivateAllItems();

        foreach (GameObject item in itemsToActivate)
        {
            ActivateItem(item);
        }
    }

    private void DeactivateAllItems()
    {
        foreach (GameObject item in enabledObjects)
        {
            item.SetActive(false);
        }
        enabledObjects.Clear();
    }

    public void ChangeMaterialColor(string target, Color color)
    {
        switch (target)
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

    public void SaveCustomization()
    {
        _isFirstInitialization = false;

        _isCameraMoving = true;

        _playerControls.Disable();

        _playerController.isCustomizing = false;

        gameObject.GetComponent<Canvas>().enabled = false;

        SetActiveParts();

        _customizePlayerData.UpdateCharacterServerRpc(activePartsIndex);

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void SetActiveParts()
    {
        for (int i = 0; i < enabledObjects.Count; i++)
        {
            activePartsIndex[i] = _customizePlayerData.allChildObjects.IndexOf(enabledObjects[i]);
        }
    }

    public void Close(InputAction.CallbackContext context)
    {
        if (!_playerController.isCustomizing || _isFirstInitialization) return;

        if (!enabledObjects.SequenceEqual(_oldEnabledObjects))
        {
            if (true)
            {
                // ShowRevertChangesPrompt()
                
                enabledObjects = new List<GameObject>(_oldEnabledObjects);
                
                SetActiveParts();
                
                _customizePlayerData.UpdateCharacterServerRpc(activePartsIndex);
            }
            //else
            //{
            //    return;
            //}
        }
        
        _isCameraMoving = true;

        _playerControls.Disable();
        _playerController.isCustomizing = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.GetComponent<Canvas>().enabled = false;
    }

    //private bool ShowRevertChangesPrompt()
    //{
    //    // Code to show a prompt and return the result, for example:
    //    // TODO: Update this with propper UI
    //    return UnityEditor.EditorUtility.DisplayDialog("You got unsaved changes", "Are you sure you want to close? Unsaved customization will be reverted.", "Yes", "No");
    //}

    public void MoveCamera(InputAction.CallbackContext context)
    {
        if (!_playerController.isCustomizing) return;

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
        if (_isMale) return;

        _facialHair.transform.GetChild(0).gameObject.SetActive(true);
        _facialHair.GetComponent<Image>().enabled = true;
        _facialHair.GetComponent<Button>().interactable = true;

        DisableEnabledObjects();
        SetDefaultMaleCharacter();

        _isMale = true;
        DestroyChildren();
        ChangeGender();
    }

    void OnClickFemaleButton()
    {
        if (!_isMale) return;
        
        _facialHair.transform.GetChild(0).gameObject.SetActive(false);
        _facialHair.GetComponent<Image>().enabled = false;
        _facialHair.GetComponent<Button>().interactable = false;

        DisableEnabledObjects();
        SetDefaultFemaleCharacter();

        _isMale = false;
        DestroyChildren();
        ChangeGender();
    }

    void OnClickHeadAllElements()
    {
        List<GameObject> elements = _isMale ? male.headAllElements : female.headAllElements;
        InstantiateButtonsForElements(elements);
    }

    private void OnClickAll_Hair()
    {
        InstantiateButtonsForElements(allGender.all_Hair);
    }

    private void OnClickEyebrow()
    {
        List<GameObject> elements = _isMale ? male.eyebrow : female.eyebrow;
        InstantiateButtonsForElements(elements);
    }

    private void OnClickFacialHair()
    {
        if (_isMale)
            InstantiateButtonsForElements(male.facialHair);
    }

    private void InstantiateButtonsForElements(List<GameObject> elements)
    {
        int counter = 1;

        DestroyChildren();

        foreach (GameObject element in elements)
        {
            Button button = Instantiate(_buttonPrefab, _options.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
            button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
            button.onClick.AddListener(() => OnClickButtonElement(element));
            counter++;
        }

        SetMouseoverOptions();
    }
    
    void OnClickButtonElement(GameObject element)
    {
        ActivateItem(element);
    }

    private void ShowColors(string materialPropName, Color[] colors, Transform parent)
    {
        foreach (Color color in colors)
        {
            Button button = Instantiate(_colorButtonPrefab, parent);
            button.GetComponent<Image>().color = color;
            button.onClick.AddListener(() => { ChangeMaterialColor(materialPropName, color); });
        }
    }

    void SkincolorShow()
    {
        ShowColors("_Color_Skin", colorSkin, _skinColors.transform);
    }

    void HaircolorShow()
    {
        ShowColors("_Color_Hair", colorHair, _hairColors.transform);
    }

    void EyecolorShow()
    {
        ShowColors("_Color_Eyes", colorEyes, _eyeColors.transform);
    }

    void BodyArtcolorShow()
    {
        ShowColors("_Color_BodyArt", colorBodyArt, _bodyArtColors.transform);
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

    void SetMouseoverOptions()
    {
        for (int i = 0; i < _options.transform.childCount; i++)
        {
            _options.transform.GetChild(i).GetComponent<MouseoverOption>().customizeUI = this;
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

}
