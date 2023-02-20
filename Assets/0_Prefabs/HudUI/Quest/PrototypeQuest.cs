using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net.NetworkInformation;
using UnityEngine.InputSystem.EnhancedTouch;

public class PrototypeQuest : MonoBehaviour
{
    private TextMeshProUGUI _questText;

    [HideInInspector] public int questID;

    [HideInInspector] public PrototypeQuestManager questManager;

    [HideInInspector] public TextMeshProUGUI questName;
    
    [HideInInspector] public TextMeshProUGUI questObjective0;
    [HideInInspector] public TextMeshProUGUI questObjective1;
    [HideInInspector] public TextMeshProUGUI questObjective2;

    [HideInInspector] public TextMeshProUGUI questObjectiveCount0;
    [HideInInspector] public TextMeshProUGUI questObjectiveCount1;
    [HideInInspector] public TextMeshProUGUI questObjectiveCount2;

    [HideInInspector] public int objectiveCount = 0;

    HashSet<int> objectives0 = new HashSet<int>();
    HashSet<int> objectives1 = new HashSet<int>();
    HashSet<int> objectives2 = new HashSet<int>();

    [HideInInspector] public int objectivesComplete0 = 0;
    [HideInInspector] public int objectivesComplete1 = 0;
    [HideInInspector] public int objectivesComplete2 = 0;

    public string questNameString;

    public string questObjectiveString0;
    public string questObjectiveString1;
    public string questObjectiveString2;

    public int questObjectiveCountMax0;
    public int questObjectiveCountMax1;
    public int questObjectiveCountMax2;

    [Header("Replace ID with with objective Count")]
    public string pingScriptCode0;
    public string pingScriptCode1;
    public string pingScriptCode2;

    private void Start()
    {
        _questText = GetComponent<TextMeshProUGUI>();
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<PrototypeQuestManager>();

        questName = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        questObjective0 = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        questObjective1 = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        questObjective2 = transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        questObjectiveCount0 = transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        questObjectiveCount1 = transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        questObjectiveCount2 = transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
        
        questName.text = questNameString;

        questObjective0.text = questObjectiveString0;
        questObjective1.text = questObjectiveString1;
        questObjective2.text = questObjectiveString2;

        questObjectiveCount0.text = "0/" + questObjectiveCountMax0;
        questObjectiveCount1.text = "0/" + questObjectiveCountMax1;
        questObjectiveCount2.text = "0/" + questObjectiveCountMax2;

        if (questObjectiveString0 != "")
        {
            questObjective0.gameObject.SetActive(true);
            questObjectiveCount0.gameObject.SetActive(true);
            objectiveCount++;
        }

        if (questObjectiveString1 != "")
        {
            questObjective1.gameObject.SetActive(true);
            questObjectiveCount1.gameObject.SetActive(true);
            objectiveCount++;
        }

        if (questObjectiveString2 != "")
        {
            questObjective2.gameObject.SetActive(true);
            questObjectiveCount2.gameObject.SetActive(true);
            objectiveCount++;
        }

        string objectName = GameObject.Find(gameObject.transform.name).name;
        pingScriptCode0 = "GameObject.Find(\"" + objectName + "\").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(0, ID" + questObjectiveCountMax0 + "); // QUEST: " + questNameString + " - " + questObjectiveString0;
        pingScriptCode1 = "GameObject.Find(\"" + objectName + "\").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(1, ID" + questObjectiveCountMax1 + "); // QUEST: " + questNameString + " - " + questObjectiveString1;
        pingScriptCode2 = "GameObject.Find(\"" + objectName + "\").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(2, ID" + questObjectiveCountMax2 + "); // QUEST: " + questNameString + " - " + questObjectiveString2;

    }

    public void QuestObjectiveUpdate(int objectiveID, int objectiveCount)
    {
        // 100 = repeatable ping
        if (objectiveCount == 100)
        {
            switch (objectiveID)
            {
                case 0:
                    if (objectivesComplete0 >= questObjectiveCountMax0) return;

                    objectivesComplete0++;
                    questObjectiveCount0.text = objectivesComplete0 + "/" + questObjectiveCountMax0;
                    break;
                case 1:
                    if (objectivesComplete1 >= questObjectiveCountMax1) return;

                    objectivesComplete1++;
                    questObjectiveCount1.text = objectivesComplete1 + "/" + questObjectiveCountMax1;
                    break;
                case 2:
                    if (objectivesComplete2 >= questObjectiveCountMax2) return;

                    objectivesComplete2++;
                    questObjectiveCount2.text = objectivesComplete2 + "/" + questObjectiveCountMax2;
                    break;
            }
        }
        else
        {
            switch (objectiveID)
            {
                case 0:
                    if (!objectives0.Add(objectiveCount) || objectivesComplete0 >= questObjectiveCountMax0) return;

                    objectivesComplete0++;
                    questObjectiveCount0.text = objectivesComplete0 + "/" + questObjectiveCountMax0;
                    break;
                case 1:
                    if (!objectives1.Add(objectiveCount) || objectivesComplete1 >= questObjectiveCountMax1) return;

                    objectivesComplete1++;
                    questObjectiveCount1.text = objectivesComplete1 + "/" + questObjectiveCountMax1;
                    break;
                case 2:
                    if (!objectives2.Add(objectiveCount) || objectivesComplete2 >= questObjectiveCountMax2) return;

                    objectivesComplete2++;
                    questObjectiveCount2.text = objectivesComplete2 + "/" + questObjectiveCountMax2;
                    break;
            }
        }
        

        



        if (objectivesComplete0 >= questObjectiveCountMax0 && objectiveID == 0)
        {
            questObjectiveCount0.color = Color.green;
        }
        
        if (objectivesComplete1 >= questObjectiveCountMax1 && objectiveID == 1)
        {
            questObjectiveCount1.color = Color.green;
        }
        
        if (objectivesComplete2 >= questObjectiveCountMax2 && objectiveID == 2)
        {
            questObjectiveCount2.color = Color.green;
        }

        if (objectivesComplete0 >= questObjectiveCountMax0 && objectivesComplete1 >= questObjectiveCountMax1 && objectivesComplete2 >= questObjectiveCountMax2)
        {
            questManager.QuestComplete(questID);
        }
    }

}
