using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Inventory/Inventory List")]

public class InventoryItemList : ScriptableObject
{

    public List<GameObject> slots;
    public List<GameObject> items;
}