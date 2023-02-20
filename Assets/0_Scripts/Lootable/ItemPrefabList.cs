using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Inventory/Item Prefab List")]

public class ItemPrefabList : ScriptableObject
{
    public List<GameObject> prefabs;
}
