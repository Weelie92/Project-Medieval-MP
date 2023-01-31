using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private GameObject _player;

    public InventoryItemList inventoryItemList;

    public GameObject inventorySlot;
    public GameObject inventoryItem;
    public GameObject inventoryTextAmount;
    
    
    public Image toolbarBG;
    public GameObject inventoryBG;
    
    public void Initialize()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        // Build toolbar/Inventory
        for (int i = 0; i < _player.GetComponent<PlayerController>().PlayerInventorySpace; i++)
        {
            GameObject tempSlot = Instantiate(inventorySlot);
            GameObject tempItem = Instantiate(inventoryItem);
            GameObject tempText = Instantiate(inventoryTextAmount);

            

            if (i < 10)
            {
                tempSlot.transform.SetParent(toolbarBG.transform);
                tempItem.transform.SetParent(tempSlot.transform);
                tempText.transform.SetParent(tempItem.transform);
            }
            else
            {
                tempSlot.transform.SetParent(inventoryBG.transform);
                tempItem.transform.SetParent(tempSlot.transform);
                tempText.transform.SetParent(tempItem.transform);
            }
            // change tempText textMeshPro to 0
            tempText.GetComponent<TextMeshProUGUI>().text = "";
            tempText.transform.position = new Vector3(5, 10, 0);

            inventoryItemList.slots[i] = tempSlot;
            inventoryItemList.items[i] = tempItem;
        }
    }

}
