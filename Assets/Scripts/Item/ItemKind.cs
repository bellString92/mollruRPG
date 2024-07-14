using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    equipItem,
    consumItem,
    materialItem
}

public abstract class ItemKind : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;
    public string description;

    public abstract void Use(BattleStat myStat);
}
