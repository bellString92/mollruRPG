using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ConsumItem", menuName = "Items/ConsumItem")]// Asset/createâ���� �������� ������Ű�� �Ҽ��ִ� �ڵ�
public class ConsumItem : ItemKind
{
    public float healAmount;

    private void OnEnable()
    {
        itemType = ItemType.consumItem;
    }

    public override void Use(BattleStat myStat) //���� player�� �ɷ�ġ�� ������ �ִ� �ڵ�
    {
        myStat.curHealPoint += healAmount;
        if (myStat.curHealPoint > myStat.maxHealPoint)
        {
            myStat.curHealPoint = myStat.maxHealPoint;
        }
    }
}
