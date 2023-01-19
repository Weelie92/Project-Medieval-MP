using UnityEngine.Events;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseoverOption : MonoBehaviour, IPointerEnterHandler
{
    public int selectedOption;
    public GameObject[] options;
    public UnityEvent onOptionSelected;

    public void OnPointerEnter(PointerEventData eventData)
    {
        for (int i = 0; i < options.Length; i++)
        {
            if (eventData.pointerEnter.gameObject == options[i])
            {
                selectedOption = i + 1;
                onOptionSelected.Invoke();
                break;
            }
        }
    }
}