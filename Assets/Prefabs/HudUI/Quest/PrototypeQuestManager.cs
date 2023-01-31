using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PrototypeQuestManager : MonoBehaviour
{
    public List<PrototypeQuest> questList;

    public Canvas prototypeQuestFeedback;
    public GameObject prototypeQuestThankyou;

    public TMP_InputField prototypeQuestFeedbackName;
    public TMP_InputField prototypeQuestFeedbackText;
    

    
    HashSet<int> quests = new HashSet<int>();

    private int _questCount = 0;

    private void Awake()
    {
        questList = new List<PrototypeQuest>();

        GameObject[] questPrefab = GameObject.FindGameObjectsWithTag("Quest");

        foreach (GameObject quest in questPrefab)
        {
            quest.GetComponent<PrototypeQuest>().questID = questList.Count;
            
            questList.Add(quest.GetComponent<PrototypeQuest>());
        }

    }

    public void QuestComplete(int quest)
    {
        if (!quests.Add(quest)) return;

        _questCount++;

        if (_questCount >= questList.Count)
        {
            Invoke("LockMouse", .3f);

            prototypeQuestFeedback.gameObject.SetActive(true);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().isTakingSurvey = true;
        }

    }

    void LockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void Send()
    {
        string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfjafwyAa9A1IFkiIqUKQSkwehyJTsXHwBeezW4BJersnur8A/formResponse";

        string gameVersion = Application.version;


        prototypeQuestThankyou.SetActive(true);

        StartCoroutine(Post(URL, gameVersion));

    }

    IEnumerator Post(string URLString, string gameVersion)
    {
        WWWForm form = new WWWForm();
        
        form.AddField("entry.2038309318", gameVersion);
        form.AddField("entry.351030009", prototypeQuestFeedbackName.text);
        form.AddField("entry.1937222637", prototypeQuestFeedbackText.text);
        

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

    public void CloseSurvey()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().isTakingSurvey = false;
        
        Cursor.lockState = CursorLockMode.Locked;

        prototypeQuestFeedback.gameObject.SetActive(false);
    }

}
