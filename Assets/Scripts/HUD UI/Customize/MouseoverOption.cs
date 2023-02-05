using UnityEngine.Events;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseoverOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    

    public CustomizeUI customizeUI;

    private void Start()
    {
        //playerCustomize = GameObject.Find("Player").GetComponent<PlayerCustomize>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (playerCustomize._movingCamera) return;
        //if (GameObject.Find("Player").GetComponent<PlayerCustomize>()._movingCamera) return;

        customizeUI.TempActivateItem(eventData.pointerEnter);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (playerCustomize._movingCamera) return;
        //if (GameObject.Find("Player").GetComponent<PlayerCustomize>()._movingCamera) return;

        customizeUI.TempActivateItem(null);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerEnter.name);
    }
       
}