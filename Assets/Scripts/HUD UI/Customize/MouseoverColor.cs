using UnityEngine.Events;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{


    public PlayerCustomize playerCustomize;

    private void Start()
    {
        playerCustomize = GameObject.Find("Player").GetComponent<PlayerCustomize>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exit");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerEnter.name);
    }

}