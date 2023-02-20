using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomizeTopBar : MonoBehaviour
{
    private Button _appearanceButton;
    private Button _traitsButton;
    private Button _skillsButton;

    private GameObject _apperancePanel;
    private GameObject _traitsPanel;
    private GameObject _skillsPanel;

    void Awake()
    {
        _appearanceButton = transform.GetChild(0).GetComponent<Button>();
        _traitsButton = transform.GetChild(1).GetComponent<Button>();
        _skillsButton = transform.GetChild(2).GetComponent<Button>();

        _apperancePanel = GameObject.Find("UI_Customize_Panels_Appearance");
        _traitsPanel = GameObject.Find("UI_Customize_Panels_Traits");
        _skillsPanel = GameObject.Find("UI_Customize_Panels_Skills");

        _appearanceButton.onClick.AddListener(() => { OnClick(_appearanceButton.name); });
        _traitsButton.onClick.AddListener(() => { OnClick(_traitsButton.name); });
        _skillsButton.onClick.AddListener(() => { OnClick(_skillsButton.name); });
    }

    public void OnClick(string buttonName)
    {
        Debug.Log(buttonName);
        // Show correct panel
        switch (buttonName)
        {
           
            case "AppearanceButton":
                _apperancePanel.gameObject.SetActive(true);
                _traitsPanel.gameObject.SetActive(false);
                _skillsPanel.gameObject.SetActive(false);

                break;
            case "TraitsButton":
                Debug.Log("Show traits panel");
                break;
            case "SkillsButton":
                Debug.Log("Show ");
                break;
        }
    }
}
