using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    public ItemKind itemKind;

    void Start()
    {
        if (itemKind != null)
        {
            Debug.Log($"ItemComponent initialized with item name: {itemKind.itemName}");
        }
    }

    public void UseItem(BattleStat myStat)
    {
        if (itemKind != null)
        {
            itemKind.Use(myStat);
        }
        else
        {
            Debug.LogWarning("No item kind assigned to this component.");
        }
    }
}
