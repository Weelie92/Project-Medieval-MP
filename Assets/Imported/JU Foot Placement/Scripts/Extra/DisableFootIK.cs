using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableFootIK : MonoBehaviour
{
    JUFootPlacement JUFootPlacementScript;
    [JUSubHeader("Press F to enable/disable Foot Placement")]
    public bool JUFootPlacementIsActive;
    public Texture2D Background;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            JUFootPlacementScript.enabled = !JUFootPlacementScript.enabled;
        }
        if(JUFootPlacementScript != null) JUFootPlacementIsActive = JUFootPlacementScript.enabled;

    }
    private void OnGUI()
    {
        if (JUFootPlacementScript == null)
        {
            JUFootPlacementScript = GetComponent<JUFootPlacement>();
        }
        else
        {
            if (JUFootPlacementIsActive) {
                GUIStyle s = new GUIStyle();
                s.normal.background = Background;
                s.fontStyle = FontStyle.Bold;
                s.fontSize = 14;
                s.alignment = TextAnchor.MiddleCenter;
                s.normal.textColor = new Color(0.5f, 1, 0.5f);
                GUI.Label(new Rect(20, 40, 300, 50), "Press [F] to disable Foot IK \n\rJU Foot Placement: " + JUFootPlacementIsActive.ToString(), s);
            }
            else
            {
                
                GUIStyle s = new GUIStyle();
                s.normal.background = Background;
                s.fontStyle = FontStyle.Bold;
                s.fontSize = 14;
                s.alignment = TextAnchor.MiddleCenter;
                s.normal.textColor = new Color(1f, 0.5f, 0.5f);
                GUI.Label(new Rect(20, 40, 300, 50), "Press [F] to enable  Foot IK \n\rJU Foot Placement: " + JUFootPlacementIsActive.ToString(), s);
            }
        }
    }
}
