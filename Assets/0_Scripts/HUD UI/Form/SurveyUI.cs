using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class SurveyUI: MonoBehaviour
{
    private PlayerController _player;

    public TextMeshProUGUI surveyHeader;

    public GameObject UI_Survey_ThankYouMessage;
    
    public string gameVersion;

    public GameObject visual;
    public int visualRating;
    
    public GameObject interaction;
    public int interactionRating;
    
    public GameObject overall;
    public int overallRating;

    public string URL;
    
    

    public void Initialize(string newURL, string newSurveyHeader)
    {       
        URL = newURL;

        UI_Survey_ThankYouMessage = GameObject.Find("UI_Survey_ThankYouMessage");

        ResetSurvey(newSurveyHeader);

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        _player.isTakingSurvey = true;

        

        Cursor.lockState = CursorLockMode.None;
    }

    public void Send()
    {
        gameVersion = Application.version;

        string visualFeedback = visual.GetComponentInChildren<TMP_InputField>().text;
        
        visualRating = visual.GetComponentInChildren<TMP_Dropdown>().value;

        string interactionFeedback = interaction.GetComponentInChildren<TMP_InputField>().text;
        interactionRating = interaction.GetComponentInChildren<TMP_Dropdown>().value;

        string overallFeedback = overall.GetComponentInChildren<TMP_InputField>().text;
        overallRating = overall.GetComponentInChildren<TMP_Dropdown>().value;

        StartCoroutine(Post(visualFeedback, visualRating, interactionFeedback, interactionRating, overallFeedback, overallRating));

        UI_Survey_ThankYouMessage.GetComponent<Canvas>().enabled = true;
    }

    public void CloseSurvey()
    {
        _player.isTakingSurvey = false;
        
        Cursor.lockState = CursorLockMode.Locked;
        
        gameObject.SetActive(false);
        
    }

 

    IEnumerator Post(string visualFeedback, int visualRating, string interactionFeedback, int interactionRating, string overallFeedback, int overallRating)
    {
        WWWForm form = new WWWForm();
        
        form.AddField("entry.2038309318", gameVersion);
        
        form.AddField("entry.189923637", visualRating);
        form.AddField("entry.1937930988", visualFeedback);

        form.AddField("entry.2027450790", interactionRating);
        form.AddField("entry.2111353711", interactionFeedback);

        form.AddField("entry.1288785535", overallRating);
        form.AddField("entry.351030009", overallFeedback);

        UnityWebRequest www = UnityWebRequest.Post(URL, form);

        yield return www.SendWebRequest();
    }

    private void ResetSurvey(string newSurveyHeader)
    {
        UI_Survey_ThankYouMessage.gameObject.GetComponent<Canvas>().enabled = false;

        gameObject.SetActive(true);


        surveyHeader.text = newSurveyHeader;

        visual.GetComponentInChildren<TMP_InputField>().text = "";
        visual.GetComponentInChildren<TMP_Dropdown>().value = 0;

        interaction.GetComponentInChildren<TMP_InputField>().text = "";
        interaction.GetComponentInChildren<TMP_Dropdown>().value = 0;

        overall.GetComponentInChildren<TMP_InputField>().text = "";
        overall.GetComponentInChildren<TMP_Dropdown>().value = 0;
    }
}

