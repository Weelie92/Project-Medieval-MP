using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowInventory : MonoBehaviour
{
    public GameObject UI_Inventory_Main;
    public GameObject UI_Inventory_Toolbar;
    
    public GameObject UI_Inventory_BG;

    private PlayerController _playerController;

    
    private void Start()
    {
        _playerController = gameObject.GetComponent<PlayerController>();

        GameObject mainUI = GameObject.Find("UI_Inventory");

        UI_Inventory_Main = mainUI.transform.Find("UI_Inventory_Main").gameObject;
        UI_Inventory_Toolbar = mainUI.transform.Find("UI_Inventory_Toolbar").gameObject;
        UI_Inventory_BG = mainUI.transform.Find("UI_Inventory_BG").gameObject;


        UI_Inventory_Main.SetActive(_playerController.isInventoryOpen);
        UI_Inventory_BG.gameObject.SetActive(_playerController.isInventoryOpen);
    }

    public void ToggleInventory()
    {
        _playerController.isInventoryOpen = !_playerController.isInventoryOpen;

        if (_playerController.isInventoryOpen)
        {
            GameObject.Find("QuestInventory").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(0, 0); // QUEST: Test Inventory - Open/Close inventory
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            GameObject.Find("QuestInventory").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(0, 1); // QUEST: Test Inventory - Open/Close inventory
            Cursor.lockState = CursorLockMode.Locked;
        }


        UI_Inventory_Main.SetActive(_playerController.isInventoryOpen);
        UI_Inventory_BG.gameObject.SetActive(_playerController.isInventoryOpen);
        
    }
}
