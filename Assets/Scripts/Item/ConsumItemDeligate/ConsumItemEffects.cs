using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ConsumItemEffects : MonoBehaviour
{
    public static void HealEffect(BattleStat myStat, ConsumItem item)
    {
        if (item.quantity > 0 && myStat.curHealPoint != myStat.maxHealPoint)
        {
            myStat.curHealPoint += item.EffectPoint;
            if (myStat.curHealPoint > myStat.maxHealPoint)
            {
                myStat.curHealPoint = myStat.maxHealPoint;
            }
            item.quantity--; // 사용 후 갯수 감소
        }
    }

    public static void AttackBoostEffect(BattleStat myStat, ConsumItem item)
    {
        if (item.quantity > 0)
        {
            Inventory.Instance.user.AllBuff(item.EffectPoint, item.EffectDuration, BuffType.Damage);
            item.quantity--; // 사용 후 갯수 감소
        }
    }

}
