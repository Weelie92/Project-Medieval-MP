using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildCustomizeUI : MonoBehaviour
{
    public Button maleButton;
    public Button femaleButton;
    public Button headAllElements;
    public Button all_Hair;
    public Button eyebrow;
    public Button facialHair;
    public Button accept;

    public Canvas options;

    public Button buttonPrefab;
    public Button colorButtonPrefab;

    public Canvas skincolors;
    public Canvas hairColors;
    public Canvas eyeColors;
    public Canvas bodyArtColors;

    public List<GameObject> maleHeadElements;
    public List<GameObject> femaleHeadElements;
    public List<GameObject> hairElements;
    public List<GameObject> maleEyebrowElements;
    public List<GameObject> femaleEyebrowElements;
    public List<GameObject> facialHairElements;

    public List<GameObject> elfEarElements;

    public PlayerCustomize playerCustomize;

    private bool isMale = true;

    private TMP_Dropdown _headDropdown;

    private void Start()
    {
        // Head dropdown
        //_headDropdown = transform.Find("HeadDropdown").GetComponent<TMP_Dropdown>();

        

        maleButton.onClick.AddListener(() => { OnClickMaleButton(); });

        femaleButton.onClick.AddListener(() => { OnClickFemaleButton(); });

        headAllElements.onClick.AddListener(() => { OnClickHeadAllElements(); });

        all_Hair.onClick.AddListener(() => { OnClickAll_Hair(); });

        eyebrow.onClick.AddListener(() => { OnClickEyebrow(); });

        facialHair.onClick.AddListener(() => { OnClickFacialHair(); });

        accept.onClick.AddListener(() => { OnClickAccept(); });


        SkincolorShow();
        HaircolorShow();
        EyecolorShow();
        BodyArtcolorShow();

        gameObject.SetActive(false);
    }

   
    void OnClickMaleButton()
    {
        if (isMale)
            return;

        GameObject.Find("Quest").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(1, 0); // QUEST
        //facialHair.gameObject.SetActive(true);
        facialHair.transform.GetChild(0).gameObject.SetActive(true);
        facialHair.GetComponent<Image>().enabled = true;
        facialHair.GetComponent<Button>().interactable = true;
        isMale = true;
        playerCustomize.isMale = true;
        DestroyChildren();
        playerCustomize.ChangeGender();

    }

    void OnClickFemaleButton()
    {
        if (!isMale)
            return;

        GameObject.Find("Quest").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(1, 1); // QUEST
        //facialHair.gameObject.SetActive(false);
        facialHair.transform.GetChild(0).gameObject.SetActive(false);
        facialHair.GetComponent<Image>().enabled = false;
        facialHair.GetComponent<Button>().interactable = false;
        isMale = false;
        playerCustomize.isMale = false;
        DestroyChildren();
        playerCustomize.ChangeGender();
    }

    void OnClickHeadAllElements()
    {
        int counter = 1;

        DestroyChildren();

        if (isMale)
        {
            foreach (GameObject element in maleHeadElements)
            {
                Button button = Instantiate(buttonPrefab, options.transform);
                button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
                button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
                button.onClick.AddListener(() => { OnClickHeadElement(element); });
                counter++;
            }
        }
        else
        {
            foreach (GameObject element in femaleHeadElements)
            {
                Button button = Instantiate(buttonPrefab, options.transform);
                button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
                button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
                button.onClick.AddListener(() => { OnClickHeadElement(element); });
                counter++;
            }
        }
    }

    void OnClickHeadElement(GameObject element)
    {
        Debug.Log(element.name);
        playerCustomize.ActivateItem(element);
    }

    void OnClickAll_Hair()
    {
        int counter = 1;

        DestroyChildren();

        foreach (GameObject element in hairElements)
        {
            Button button = Instantiate(buttonPrefab, options.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
            button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
            button.onClick.AddListener(() => { OnClickHairElement(element); });
            counter++;
        }
    }


    void OnClickHairElement(GameObject element)
    {
        Debug.Log(element.name);
        playerCustomize.ActivateItem(element);
    }

    void OnClickEyebrow()
    {
        gameObject.GetComponent<ProtoTypeQuestPing>().PING(2);

        int counter = 1;

        DestroyChildren();

        if (isMale)
        {
            foreach (GameObject element in maleEyebrowElements)
            {
                Button button = Instantiate(buttonPrefab, options.transform);
                button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
                button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
                button.onClick.AddListener(() => { OnClickEyebrowElement(element); });
                counter++;
            }
        }
        else
        {
            foreach (GameObject element in femaleEyebrowElements)
            {
                Button button = Instantiate(buttonPrefab, options.transform);
                button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
                button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
                button.onClick.AddListener(() => { OnClickEyebrowElement(element); });
                counter++;
            }
        }
    }

    void OnClickEyebrowElement(GameObject element)
    {
        Debug.Log(element.name);
        playerCustomize.ActivateItem(element);
    }


    void OnClickFacialHair()
    {
        if (!isMale)
            return;

        int counter = 1;

        DestroyChildren();

        foreach (GameObject element in facialHairElements)
        {
            Button button = Instantiate(buttonPrefab, options.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
            button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
            button.onClick.AddListener(() => { OnClickFacialHairElement(element); });
            counter++;
        }
    }

    void OnClickFacialHairElement(GameObject element)
    {
        Debug.Log(element.name);
        playerCustomize.ActivateItem(element);
    }

    void OnClickAccept()
    {
        playerCustomize.SaveCustomization();
    }

    void OnClickElf_Ear(bool value)
    {
        int counter = 1;

        DestroyChildren();

        foreach (GameObject element in elfEarElements)
        {
            Button button = Instantiate(buttonPrefab, options.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
            button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
            button.onClick.AddListener(() => { OnClickElf_EarElement(element); });
            counter++;
        }
    }

    void OnClickElf_EarElement(GameObject element)
    {
        Debug.Log(element.name);
        playerCustomize.ActivateItem(element);
    }

    void SkincolorShow()
    {
        foreach (Color color in playerCustomize.colorSkin)
        {
            Button button = Instantiate(colorButtonPrefab, skincolors.transform);
            button.GetComponent<Image>().color = color;
            button.onClick.AddListener(() => { playerCustomize.ChangeMaterialColor("_Color_Skin", color); });
        }
    }

    void HaircolorShow()
    {
        foreach (Color color in playerCustomize.colorHair)
        {
            Button button = Instantiate(colorButtonPrefab, hairColors.transform);
            button.GetComponent<Image>().color = color;
            button.onClick.AddListener(() => { playerCustomize.ChangeMaterialColor("_Color_Hair", color); });
        }
    }

    void EyecolorShow()
    {
        foreach (Color color in playerCustomize.colorEyes)
        {
            Button button = Instantiate(colorButtonPrefab, eyeColors.transform);
            button.GetComponent<Image>().color = color;
            button.onClick.AddListener(() => { playerCustomize.ChangeMaterialColor("_Color_Eyes", color); });
        }
    }

    void BodyArtcolorShow()
    {
        foreach (Color color in playerCustomize.colorBodyArt)
        {
            Button button = Instantiate(colorButtonPrefab, bodyArtColors.transform);
            button.GetComponent<Image>().color = color;
            button.onClick.AddListener(() => { playerCustomize.ChangeMaterialColor("_Color_BodyArt", color); });
        }
    }



    void DestroyChildren()
    {
        for (int i = options.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = options.transform.GetChild(i);
            // code to remove the child
            Destroy(child.gameObject);
        }
        
    }
    

}
