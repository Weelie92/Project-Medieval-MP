using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool isInteractable = true;
    public Vector3 lookAtRotation;

    public Canvas _interactE;
    public bool showInteractE = false;

    [Header("Survey")]
    public GameObject formSurvey;
    public string newSurveyHeader;
    public string URL;

    [Header("Customize")]
    [SerializeField] private Vector3 _cameraPositionWhenCustomizing;
    [SerializeField] private Vector3 _playerPositionWhenCustomizing;
    [SerializeField] private float _playerRotationWhenCustomizing;
    [SerializeField] private GameObject _UI_Customize;


    private void Start()
    {
        _interactE = GetComponentInChildren<Canvas>();
        _interactE.enabled = showInteractE;
    }

    public void Interact(GameObject player)
    {
        switch (gameObject.tag)
        {
            case "Door":
                break;
            case "Customize":
                GameObject.Find("QuestCustomize").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(0, 0); // QUEST: Test Customization - Open/Close customization

                PlayerCustomize playerCustomize = player.GetComponent<PlayerCustomize>();

                _UI_Customize.GetComponent<Canvas>().enabled = true;
                _UI_Customize.GetComponent<CustomizeUI>().playerCustomize = playerCustomize;
                _UI_Customize.GetComponent<CustomizeUI>().Initialize();

                playerCustomize.enabled = true;
                playerCustomize.cameraPositionWhenCustomizing = _cameraPositionWhenCustomizing;
                playerCustomize.playerPositionWhenCustomizing = _playerPositionWhenCustomizing;
                playerCustomize.playerRotationWhenCustomizing = _playerRotationWhenCustomizing;
                playerCustomize.Initialize();
                player.GetComponent<PlayerController>().isCustomizing = true;
                break;
            case "Lootable":
                _interactE.enabled = false;
                player.GetComponent<PlayerController>().LootItem(gameObject.GetComponent<Item>());
                break;
            case "Form":
                _interactE.enabled = false;

                if (!isInteractable) return;

                isInteractable = false;
                
                formSurvey.SetActive(true);
                formSurvey.GetComponent<SurveyUI>().Initialize(URL, newSurveyHeader);
                
                break;
            default:
                break;
        }

        
        
    }
}
