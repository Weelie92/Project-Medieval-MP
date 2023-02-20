using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool isInteractable { get; set; } = true;
    public Vector3 lookAtRotation { get; set; }

    public Canvas _interactE { get; private set; }
    public bool showInteractE { get; set; }

    public GameObject formSurvey { get; set; }
    public string newSurveyHeader { get; set; }
    public string URL { get; set; }

    [Header("Customize")]
    [SerializeField] private Vector3 _cameraPositionWhenCustomizing;
    [SerializeField] private Vector3 _playerPositionWhenCustomizing;
    [SerializeField] private float _playerRotationWhenCustomizing;


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
                var customizeUI = GameObject.FindGameObjectWithTag("UI_Customize").GetComponent<CustomizeUI>();
                customizeUI.Initialize();
                break;
            case "Form":
                _interactE.enabled = false;

                if (!isInteractable) return;

                isInteractable = false;

                formSurvey.SetActive(true);
                var surveyUI = formSurvey.GetComponent<SurveyUI>();
                surveyUI.Initialize(URL, newSurveyHeader);

                break;
            default:
                break;
        }
    }
}