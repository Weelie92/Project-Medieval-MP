using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoTypeQuestPing : MonoBehaviour
{
    public PrototypeQuest quest;
    public int pingID;

    public string pingScriptCode;

    public int pingCount = 0;
    public int pingCountMax;

    HashSet<int> pinged = new HashSet<int>();

    private void Awake()
    {
        string objectName = GameObject.Find(gameObject.transform.name).name;
        pingScriptCode = "GameObject.Find(\"" + objectName + "\").GetComponent<ProtoTypeQuestPing>().PING(Replace: " + pingCountMax + "); // QUEST";
    }
    

    public void PING(int ping)
    {
        Debug.Log(ping);
        if (!pinged.Add(ping)) return;
        Debug.Log("PINGED");

        pingCount++;
        
        //quest.QuestObjectiveUpdate(pingCount);
        
        if (pingCount >= pingCountMax)
        {
            //quest.QuestObjectiveComplete(pingID);
        }
    }
}
