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
        maxStack= 99;
    }
    public ConsumItem(ConsumItem original) : base(original)
    {
        
    }

    public override void Use(BattleStat myStat) //���� player�� �ɷ�ġ�� ������ �ִ� �ڵ�
    {
        if (quantity > 0)
        {
            myStat.curHealPoint += healAmount;
            if (myStat.curHealPoint > myStat.maxHealPoint)
            {
                myStat.curHealPoint = myStat.maxHealPoint;
            }
            quantity--; // ��� �� ���� ����
        }        
    }
}
