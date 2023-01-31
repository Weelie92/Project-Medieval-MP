using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDUIController : MonoBehaviour
{

    [SerializeField] private GameObject _UI_PauseMenu;
    [SerializeField] private GameObject _UI_Customize;
    [SerializeField] private GameObject _UI_Inventory;
    [SerializeField] private GameObject _UI_Survey;
    [SerializeField] private GameObject _UI_Quest;
    [SerializeField] private GameObject _UI_Quest_Feedbackform;
    [SerializeField] private GameObject _UI_NetworkManager;

    private void Start()
    {
        _UI_PauseMenu.GetComponent<Canvas>().enabled = false;
        _UI_Customize.GetComponent<Canvas>().enabled = false;
        _UI_Inventory.GetComponent<Canvas>().enabled = false;
        _UI_Survey.GetComponent<Canvas>().enabled = false;
        _UI_Quest.GetComponent<Canvas>().enabled = false;
        _UI_Quest_Feedbackform.GetComponent<Canvas>().enabled = false;
    }
}
