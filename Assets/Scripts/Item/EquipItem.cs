using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EquipItem", menuName = "Items/EquipItem")]// createâ�� ����
public class EquipItem : ItemKind
{
    public float attackBoost;
    public float maxHealBoost;

    private void OnEnable()
    {
        itemType = ItemType.equipItem;
    }

    public override void Use(BattleStat myStat)
    {
        myStat.AttackPoint += attackBoost;
        myStat.maxHealPoint += maxHealBoost;
    }
}
