using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildInventory : MonoBehaviour
{
    private PlayerStats _player;

    public InventoryItemList inventoryItemList;

    public GameObject inventorySlot;
    public GameObject inventoryItem;
    public GameObject inventoryTextAmount;
    
    
    public Image toolbarBG;
    public Image inventoryBG;

    private void Start()
    {
        _player = _player = GameObject.Find("Player").GetComponent<PlayerStats>();

        // Build toolbar/Inventory
        for (int i = 0; i < _player.PlayerInventorySpace; i++)
        {
            GameObject tempBG = Instantiate(inventorySlot);
            GameObject tempItem = Instantiate(inventoryItem);
            GameObject tempText = Instantiate(inventoryTextAmount);

            

            if (i < 10)
            {
                tempBG.transform.SetParent(toolbarBG.transform);
                tempItem.transform.SetParent(tempBG.transform);
                tempText.transform.SetParent(tempItem.transform);
            }
            else
            {
                tempBG.transform.SetParent(inventoryBG.transform);
                tempItem.transform.SetParent(tempBG.transform);
                tempText.transform.SetParent(tempItem.transform);
            }
            // change tempText textMeshPro to 0
            tempText.GetComponent<TextMeshProUGUI>().text = "";
            tempText.transform.position = new Vector3(5, 10, 0);

            inventoryItemList.slots[i] = tempBG;
            inventoryItemList.items[i] = tempItem;
        }
    }

}
