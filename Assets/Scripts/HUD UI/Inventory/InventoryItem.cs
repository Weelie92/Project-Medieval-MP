using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Reflection;
using TMPro;


public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image image;
    
    public Sprite sprite;

    public TextMeshProUGUI _inventoryTextAmount;

    

    public int stackSize = 0;
    public int maxStackSize;
    
    public string itemName = "Empty";


    [HideInInspector] public GameObject oldParent;

    [HideInInspector] public GameObject currentSlot = null;


    private void Start()
    {
        image.sprite = sprite;
        _inventoryTextAmount = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _inventoryTextAmount.transform.position = new Vector3(5, 10, 0);

    }

    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemName == "Empty") return;


        image.raycastTarget = false;
        _inventoryTextAmount.raycastTarget = false;
        oldParent = transform.parent.gameObject;
        transform.SetParent(GameObject.FindGameObjectWithTag("TopLayer").transform);
        
        //originalSlot = transform.parent.gameObject;
        //transform.SetParent(transform.parent);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (itemName == "Empty") return;

        if (!eventData.pointerCurrentRaycast.gameObject)
        {
            transform.position = Input.mousePosition;
            return;
        }



        transform.position = Input.mousePosition;
        // New switch state
        currentSlot = eventData.pointerCurrentRaycast.gameObject.tag switch
        {
            "InventorySlot" => eventData.pointerCurrentRaycast.gameObject,
            "InventoryItem" => eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject,
            "InventoryTextAmount" => eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.gameObject,
            _ => null,
        };

    }

    public void OnEndDrag(PointerEventData eventData)
    {


        image.raycastTarget = true;
        _inventoryTextAmount.raycastTarget = true;


        if (currentSlot)
        {
            GameObject.Find("QuestInventory").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(2, 0); // QUEST: Test Inventory - Move something
            
            transform.SetParent(currentSlot.transform);
            transform.position = currentSlot.transform.position;
            currentSlot.GetComponentInChildren<InventoryItem>().ChangeParentSlot(oldParent.transform);
        }
        else
        {
            transform.SetParent(oldParent.transform);
            transform.position = oldParent.transform.position;

        }
    }

    public void UpdateItem()
    {
        if (stackSize > 1)
        {
            _inventoryTextAmount.text = stackSize.ToString();
        }
        else
        {
            _inventoryTextAmount.text = "";
        }
    }

    public void ChangeParentSlot(Transform newParent)
    {
        transform.SetParent(newParent);
        transform.position = newParent.position;
    }

   

    public void SetStackSize(Item item) 
    {
        maxStackSize = item.stackSize;
        image.sprite = item.itemSprite;
        //_inventoryTextAmount.transform.position = new Vector3(0, 5, 0);
    }

   

}
