using UnityEngine.Events;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseoverOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    

    public PlayerCustomize playerCustomize;

    private void Start()
    {
        playerCustomize = GameObject.Find("Player").GetComponent<PlayerCustomize>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter");
        playerCustomize.TempActivateItem(eventData.pointerEnter);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exit");
        playerCustomize.TempActivateItem(null);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerEnter.name);
    }
       
}