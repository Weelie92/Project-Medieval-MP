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

    public GameObject player;

    [Header("Survey")]
    public GameObject formSurvey;
    public string newSurveyHeader;
    public string URL;


    private void Start()
    {
        _interactE = GetComponentInChildren<Canvas>();
        _interactE.enabled = showInteractE;

        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Interact()
    {
        switch (gameObject.tag)
        {
            case "Door":
                break;
            case "Customize":
                GameObject.Find("Quest").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(0, 1); // QUEST
                player.GetComponent<PlayerCustomize>().enabled = true;
                player.GetComponent<PlayerCustomize>().Initialize(lookAtRotation);
                break;
            case "Lootable":
                _interactE.enabled = false;
                player.GetComponent<PlayerController>().LootItem(gameObject.GetComponent<Item>());
                break;
            case "Form":
                _interactE.enabled = false;

                if (!isInteractable) return;

                isInteractable = false;

                player.GetComponent<PlayerController>()._isTakingSurvey = true;
                
                formSurvey.SetActive(true);
                formSurvey.GetComponent<FormBuilder>().Initialize(URL, newSurveyHeader);                
                break;
            default:
                break;
        }

        
        
    }
}
