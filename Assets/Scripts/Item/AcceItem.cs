using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AcceItem", menuName = "Items/AcceItem")]// Asset/createâ���� �������� ������Ű�� �Ҽ��ִ� �ڵ�
public class AcceItem : ItemKind
{
    public float attackBoost;
    public float maxHealBoost;

    private void OnEnable()
    {
        itemType = ItemType.acceItem;
    }

    public override void Use(BattleStat myStat) //���� player�� �ɷ�ġ�� ������ �ִ� �ڵ�
    {
        myStat.AttackPoint += attackBoost;
        myStat.maxHealPoint += maxHealBoost;
        myStat.curHealPoint += maxHealBoost;
    }
}
