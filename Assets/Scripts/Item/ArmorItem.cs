using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ArmorItem", menuName = "Items/ArmorItem")]// Asset/create창에서 아이템을 생성시키게 할수있는 코드
public class ArmorItem : ItemKind
{
    public float maxHealBoost;

    private void OnEnable()
    {
        itemType = ItemType.armorItem;
    }

    public override void Use(BattleStat myStat) //사용시 player의 능력치에 영향을 주는 코드
    {
        myStat.maxHealPoint += maxHealBoost;
        myStat.curHealPoint += maxHealBoost;
    }
}
