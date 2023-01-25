using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Prefab List")]

public class ItemPrefabList : ScriptableObject
{
    public List<GameObject> prefabs;
}
