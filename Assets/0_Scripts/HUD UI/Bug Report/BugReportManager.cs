using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class BugReportManager : MonoBehaviour
{
    public GameObject bugReportMenu;
    public GameObject bugReportThankYou;

    public TMP_InputField bugReportFeedbackID;
    public TMP_InputField bugReportFeedbackText;


    private void Awake()
    {

        bugReportMenu.SetActive(false);
    }
    
    public void ToggleBugReportCanvas()
    {
        if (bugReportMenu.activeSelf)
        {
            bugReportMenu.SetActive(false);
        }
        else
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().isTakingSurvey = true;
            ResetBugReport();
            bugReportMenu.SetActive(true);
            
        }
    }

    public void Send()
    {
        string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdzvL7Bmj_mPTvFciXOxEb-0ax9uJYXITyt0PAsF-5LS25oFw/formResponse";

        string gameVersion = Application.version;


        bugReportThankYou.SetActive(true);

        StartCoroutine(Post(URL, gameVersion));

    }

    IEnumerator Post(string URLString, string gameVersion)
    {
        WWWForm form = new WWWForm();

        form.AddField("entry.1895227861", gameVersion);
        form.AddField("entry.1042383331", bugReportFeedbackID.text);
        form.AddField("entry.174846864", bugReportFeedbackText.text);


        UnityWebRequest www = UnityWebRequest.Post(URLString, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }
    
    private void ResetBugReport()
    {
        bugReportFeedbackID.text = "";
        bugReportFeedbackText.text = "";


        bugReportThankYou.SetActive(false);

    }


    public void CloseBugReport()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().isTakingSurvey = false;

        bugReportMenu.SetActive(false);
    }
}
