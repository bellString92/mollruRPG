using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ArmorItem", menuName = "Items/ArmorItem")]// Asset/createâ���� �������� ������Ű�� �Ҽ��ִ� �ڵ�
public class ArmorItem : ItemKind
{
    public float maxHealBoost;

    private void OnEnable()
    {
        itemType = ItemType.armorItem;
        maxStack = 1;
    }
    public ArmorItem(ArmorItem original) : base(original)
    {
        maxHealBoost = original.maxHealBoost;
    }

    public override void Use(BattleStat myStat) //���� player�� �ɷ�ġ�� ������ �ִ� �ڵ�
    {
        myStat.maxHealPoint += maxHealBoost;
        myStat.curHealPoint += maxHealBoost;
    }
}
