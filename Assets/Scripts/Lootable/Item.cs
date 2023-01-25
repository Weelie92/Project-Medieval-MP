using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public Sprite itemSprite;
    public Sprite emptySprite;

    
    public enum ItemType { Weapon, Armor, Consumable, QuestItem, Misc, Resource, KeyItem, Currency }
    public ItemType itemType;

    [HideInInspector] public int stackSize;

    public bool isLootable = true;

    private void Awake()
    {
        switch (itemType)
        {
            case ItemType.Weapon:
                stackSize = 1;
                break;
            case ItemType.Armor:
                stackSize = 1;
                break;
            case ItemType.Consumable:
                stackSize = 5;
                break;
            case ItemType.QuestItem:
                stackSize = 1;
                break;
            case ItemType.Misc:
                stackSize = 1;
                break;
            case ItemType.Resource:
                stackSize = 99;
                break;
            case ItemType.KeyItem:
                stackSize = 1;
                break;
            case ItemType.Currency:
                stackSize = 99999;
                break;
            default:
                break;
        }

        if (itemSprite == null)
        {
            itemSprite = emptySprite;
        }
    }
}
