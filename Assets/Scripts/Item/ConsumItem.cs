using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ConsumItem", menuName = "Items/ConsumItem")]// Asset/create창에서 아이템을 생성시키게 할수있는 코드
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

    public override void Use(BattleStat myStat) //사용시 player의 능력치에 영향을 주는 코드
    {
        if (quantity > 0)
        {
            myStat.curHealPoint += healAmount;
            if (myStat.curHealPoint > myStat.maxHealPoint)
            {
                myStat.curHealPoint = myStat.maxHealPoint;
            }
            quantity--; // 사용 후 갯수 감소
        }        
    }
}
