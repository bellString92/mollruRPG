using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EquipItem", menuName = "Items/EquipItem")]// create창에서 아이템을 생성시키게 할수있는 코드
public class EquipItem : ItemKind
{
    public float attackBoost;
    public float maxHealBoost;

    private void OnEnable()
    {
        itemType = ItemType.equipItem;
    }

    public override void Use(BattleStat myStat) //사용시 player의 능력치에 영향을 주는 코드
    {
        myStat.AttackPoint += attackBoost;
        myStat.maxHealPoint += maxHealBoost;
    }
}
