using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ConsumItem", menuName = "Items/ConsumItem")]
public class ConsumItem : ItemKind
{
    public float healAmount;

    private void OnEnable()
    {
        itemType = ItemType.consumItem;
    }

    public override void Use(BattleStat myStat)
    {
        myStat.curHealPoint += healAmount;
        if (myStat.curHealPoint > myStat.maxHealPoint)
        {
            myStat.curHealPoint = myStat.maxHealPoint;
        }
    }
}
