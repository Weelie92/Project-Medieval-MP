using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using PsychoticLab;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using static Cinemachine.AxisState;


public class PlayerCustomize : MonoBehaviour
{
    private GameObject _player;
    private Animator _playerAnimator;
    private PlayerControls _playerControls;
    private CinemachineVirtualCamera _vcam;
    private Cinemachine3rdPersonFollow _vcamFollow;
    private PlayerAiming _playerAiming;

    public Transform _lookAt;
    [SerializeField] private Cinemachine.AxisState xAxis;
    [SerializeField] private Cinemachine.AxisState yAxis;
    private Cinemachine.CinemachineInputProvider inputAxisProvider;

    private Quaternion _targetRotation;
    private bool _cameraMoving = false;
    private bool _movingCamera = false;

    void Awake()
    {
        _playerControls = new PlayerControls();
        _playerAiming = GetComponent<PlayerAiming>();
        _playerAnimator = GetComponent<Animator>();

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
        


        _playerControls.Enable();

        _playerControls.Customize.Close.started += Close;
        _playerControls.Customize.Close.canceled += Close;

        _playerControls.Customize.MoveCamera.started += MoveCamera;
        _playerControls.Customize.MoveCamera.canceled += MoveCamera;

        _vcam = GetComponentInChildren<CinemachineVirtualCamera>();

        inputAxisProvider = GetComponent<Cinemachine.CinemachineInputProvider>();
        xAxis.SetInputAxisProvider(0, inputAxisProvider);
        yAxis.SetInputAxisProvider(1, inputAxisProvider);

        Debug.Log("One");
    }

    /*  
     *  Camera distance 1.7f
     *  Shoulder offset (0, -1, 0)    
    */

    public void OnPointerEnter(PointerEventData data)
    {
        Debug.Log("Two");
    }

    public void Initialize(Vector3 position)
    {
        // reset script values
        Debug.Log("Initialize");
        
        _vcamFollow = _vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        _player = GameObject.Find("Player");
        _player.GetComponent<PlayerController>()._isCustomizing = true;
        _player.GetComponent<PlayerController>().enabled = false;
        _player.GetComponent<HeadLookAtDirection>().enabled = false;
        _player.GetComponent<PlayerAiming>().enabled = false;


        _playerAnimator.SetBool("isMoving", false);
        _playerAnimator.SetBool("isRunning", false);
        _playerAnimator.SetBool("isOffhanding", false);
        _playerAnimator.SetBool("isFalling", false);



        xAxis.Value = 0;
        yAxis.Value = 0;

        _targetRotation = Quaternion.Euler(position);
        

        _cameraMoving = true;

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
                //_vcam.transform.position = Vector3.Lerp(_mainCameraPos, _cameraPosition, Time.deltaTime * 5f);
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
            //_vcam.transform.position = Vector3.Lerp(_mainCameraPos, _cameraPosition, Time.deltaTime * 5f);
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

            // collect the material for the random character, only if null in the inspector;
            // TODO: WHAT
            //if (!mat)
            //{
            //    if (go.GetComponent<SkinnedMeshRenderer>())
            //        mat = go.GetComponent<SkinnedMeshRenderer>().material;
            //}
        }
    }

    void ActivateItem(GameObject go)
    {
        // enable item
        go.SetActive(true);

        // add item to the enabled items list
        enabledObjects.Add(go);
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

    void CameraStop()
    {
        _cameraMoving = false;
    }

    





    public void Close(InputAction.CallbackContext context)
    {
        if (_player == null) return;
        
        switch (context.phase)
        {
            case InputActionPhase.Started:
                _player.GetComponent<PlayerController>()._isCustomizing = false;
                Invoke(nameof(Disable), 1f);
                break;
        }
    }

    public void MoveCamera(InputAction.CallbackContext context)
    {
        if (_player == null) return;

        switch (context.phase)
        {
            case InputActionPhase.Started:
                _movingCamera = true;
                break;
            case InputActionPhase.Canceled:
                _movingCamera = false;
                break;
        }
    }

    void Disable()
    {
        _player.GetComponent<PlayerController>().enabled = true;
        _player.GetComponent<HeadLookAtDirection>().enabled = true;
        _player.GetComponent<PlayerAiming>().enabled = true;
        _player.GetComponent<PlayerAiming>().xAxis.Value = 0;
        _player.GetComponent<PlayerAiming>().yAxis.Value = 0;
        enabled = false;
    }
}
