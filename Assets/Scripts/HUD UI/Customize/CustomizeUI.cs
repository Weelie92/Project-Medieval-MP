using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomizeUI : MonoBehaviour
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

    
    public void Initialize()
    {
        DestroyColorChildren();

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
        
        GameObject.Find("QuestCustomize").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(1, 1); // QUEST: Test Customization - Open/Close customization
        
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
        GameObject.Find("QuestCustomize").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(1, 0); // QUEST: Test Customization - Open/Close customization
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

            foreach (GameObject element in playerCustomize.male.headAllElements)
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
            foreach (GameObject element in playerCustomize.female.headAllElements)
            {
                Button button = Instantiate(buttonPrefab, options.transform);
                button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
                button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
                button.onClick.AddListener(() => { OnClickHeadElement(element); });
                counter++;
            }
        }

        SetMouseoverOptions();
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

        foreach (GameObject element in playerCustomize.allGender.all_Hair)
        {
            Button button = Instantiate(buttonPrefab, options.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
            button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
            button.onClick.AddListener(() => { OnClickHairElement(element); });
            counter++;
        }

        SetMouseoverOptions();
    }


    void OnClickHairElement(GameObject element)
    {
        playerCustomize.ActivateItem(element);
    }

    void OnClickEyebrow()
    {
        int counter = 1;

        DestroyChildren();

        if (isMale)
        {
            foreach (GameObject element in playerCustomize.male.eyebrow)
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
            foreach (GameObject element in playerCustomize.female.eyebrow)
            {
                Button button = Instantiate(buttonPrefab, options.transform);
                button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
                button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
                button.onClick.AddListener(() => { OnClickEyebrowElement(element); });
                counter++;
            }
        }

        SetMouseoverOptions();
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

        foreach (GameObject element in playerCustomize.male.facialHair)
        {
            Button button = Instantiate(buttonPrefab, options.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().name = element.name;
            button.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();
            button.onClick.AddListener(() => { OnClickFacialHairElement(element); });
            counter++;
        }
        
        SetMouseoverOptions();
    }

    void OnClickFacialHairElement(GameObject element)
    {
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

        SetMouseoverOptions();
    }

    void OnClickElf_EarElement(GameObject element)
    {
        Debug.Log(element.name);
        playerCustomize.ActivateItem(element);
    }

    void SkincolorShow()
    {
        foreach (Color color in colorSkin)
        {
            Button button = Instantiate(colorButtonPrefab, skincolors.transform);
            button.GetComponent<Image>().color = color;
            button.onClick.AddListener(() => { playerCustomize.ChangeMaterialColor("_Color_Skin", color); });
        }
    }

    void HaircolorShow()
    {
        foreach (Color color in colorHair)
        {
            Button button = Instantiate(colorButtonPrefab, hairColors.transform);
            button.GetComponent<Image>().color = color;
            button.onClick.AddListener(() => { playerCustomize.ChangeMaterialColor("_Color_Hair", color); });
        }
    }

    void EyecolorShow()
    {
        foreach (Color color in colorEyes)
        {
            Button button = Instantiate(colorButtonPrefab, eyeColors.transform);
            button.GetComponent<Image>().color = color;
            button.onClick.AddListener(() => { playerCustomize.ChangeMaterialColor("_Color_Eyes", color); });
        }
    }

    void BodyArtcolorShow()
    {
        foreach (Color color in colorBodyArt)
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

    void DestroyColorChildren()
    {
        for (int i = skincolors.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = skincolors.transform.GetChild(i);
            // code to remove the child
            Destroy(child.gameObject);
        }

        for (int i = hairColors.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = hairColors.transform.GetChild(i);
            // code to remove the child
            Destroy(child.gameObject);
        }

        for (int i = eyeColors.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = eyeColors.transform.GetChild(i);
            // code to remove the child
            Destroy(child.gameObject);
        }

        for (int i = bodyArtColors.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = bodyArtColors.transform.GetChild(i);
            // code to remove the child
            Destroy(child.gameObject);
        }
    }

    void SetMouseoverOptions()
    {
        for (int i = 0; i < options.transform.childCount; i++)
        {
            options.transform.GetChild(i).GetComponent<MouseoverOption>().playerCustomize = playerCustomize;
        }
    }
    

}
