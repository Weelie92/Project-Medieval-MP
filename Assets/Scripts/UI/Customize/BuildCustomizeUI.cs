using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildCustomizeUI : MonoBehaviour
{
    public Button maleButton;
    public Button femaleButton;
    public Button headAllElements;
    public Button all_Hair;
    public Button eyebrow;
    public Button facialHair;

    public Toggle elf_Ear;

    public List<GameObject> maleHeadElements;
    public List<GameObject> femaleHeadElements;
    public List<GameObject> hairElements;
    public List<GameObject> maleEyebrowElements;
    public List<GameObject> femaleEyebrowElements;
    public List<GameObject> facialHairElements;

    public List<GameObject> elfEarElements;

    private void Start()
    {
        maleButton.onClick.AddListener(() => { OnClickMaleButton(); });

        femaleButton.onClick.AddListener(() => { OnClickFemaleButton(); });

        headAllElements.onClick.AddListener(() => { OnClickHeadAllElements(); });

        all_Hair.onClick.AddListener(() => { OnClickAll_Hair(); });

        eyebrow.onClick.AddListener(() => { OnClickEyebrow(); });

        facialHair.onClick.AddListener(() => { OnClickFacialHair(); });

        elf_Ear.onValueChanged.AddListener((bool value) => { OnClickElf_Ear(value); });
    }

    void OnClickMaleButton()
    {
        
    }

    void OnClickFemaleButton()
    {
        
    }

    void OnClickHeadAllElements()
    { 
        
    }

    void OnClickAll_Hair()
    {

    }

    void OnClickEyebrow()
    {

    }

    void OnClickFacialHair()
    {

    }

    void OnClickElf_Ear(bool value)
    {

    }
    

}
