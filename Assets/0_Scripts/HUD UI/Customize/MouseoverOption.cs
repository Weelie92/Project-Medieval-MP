using UnityEngine.Events;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseoverOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    

    public CustomizeUI customizeUI;


    public void OnPointerEnter(PointerEventData eventData)
    {
        customizeUI.ActivateTempItem(eventData.pointerEnter);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        customizeUI.ActivateTempItem(null);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerEnter.name);
    }
       
}