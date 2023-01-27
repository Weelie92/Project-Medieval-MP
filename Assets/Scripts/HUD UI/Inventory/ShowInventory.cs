using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowInventory : MonoBehaviour
{
    public GameObject mainInventory;
    public GameObject toolbar;
    
    public Image blackBackground;

    public bool showInventory = false;

    public void ToggleInventory()
    {
        showInventory = !showInventory;

        if (showInventory)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        gameObject.GetComponent<PlayerAiming>().enabled = !showInventory;

        mainInventory.SetActive(showInventory);
        blackBackground.gameObject.SetActive(showInventory);
        
    }
}
