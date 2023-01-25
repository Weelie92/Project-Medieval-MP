using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowInventory : MonoBehaviour
{
    public Image mainInventory;

    private bool _showInventory = true;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleInventory()
    {
        mainInventory.gameObject.SetActive(_showInventory);
        _showInventory = !_showInventory;
    }
}
