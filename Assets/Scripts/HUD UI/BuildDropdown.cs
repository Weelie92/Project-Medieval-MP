using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class BuildDropdown : MonoBehaviour, IPointerMoveHandler
{
    public List<GameObject> options = new List<GameObject>();
    [SerializeField] private List<GameObject> activeOptions = new List<GameObject>();
    public bool optionsMultipleGenders;

    private int counter = 0;

    private int _mouseoverOptionIndex = 0;
    private TMP_Dropdown _dropdown;
    private PlayerCustomize _playerCustomize;


    void Start()
    {
        _dropdown = GetComponent<TMP_Dropdown>();


        _playerCustomize = GameObject.Find("Player").GetComponent<PlayerCustomize>();

        _dropdown.onValueChanged.AddListener(OnValueChanged);

        UpdateDropdownOptions();
        // Add a OnPointerEnter to every option in _dropdown


    }

    public void OnPointerMove(PointerEventData eventData)
    {
        var enteredOption = eventData.pointerEnter;
        string parentName = enteredOption.transform.parent.name;



        //if (parentName[parentName.Length - 1].ToString() == " ")
        //{
        //    _mouseoverOptionIndex = parentName[parentName.Length - 1];
        //}

        // Debug log what comes after : in a string


        Debug.Log(enteredOption.transform.parent);
        DisplayMouseoverOption();
    }

    void DisplayMouseoverOption()
    {

    }

    void OnValueChanged(int optionPicked)
    {
        if (optionsMultipleGenders)
        {
            _playerCustomize.ActivateItem(activeOptions[optionPicked]);
        }
        else
        {
            _playerCustomize.ActivateItem(options[optionPicked]);

        }
    }
    
    public void UpdateDropdownOptions()
    {
        

        _dropdown.ClearOptions();

        if (optionsMultipleGenders)
        {
            if (_playerCustomize.isMale)
            {
                activeOptions = options.FindAll(x => x.name.Contains("Male"));
            }
            else
            {
                activeOptions = options.FindAll(x => x.name.Contains("Female"));
            }

            foreach (GameObject option in activeOptions)
            {
                _dropdown.options.Add(new TMP_Dropdown.OptionData(counter++.ToString()));
            }
        }
        else
        {
                
            foreach (GameObject option in options)
            {
                _dropdown.options.Add(new TMP_Dropdown.OptionData(counter++.ToString())) ;
            }
        }

        counter = 0;

    }
}
