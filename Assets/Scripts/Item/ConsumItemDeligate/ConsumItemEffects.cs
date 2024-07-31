using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ConsumItemEffects : MonoBehaviour
{
    public static void HealEffect(Player user, ConsumItem item)
    {
        if (item.quantity > 0 && user.myStat.curHealPoint != user.myStat.maxHealPoint)
        {
            user.myStat.curHealPoint += item.EffectPoint;
            if (user.myStat.curHealPoint > user.myStat.maxHealPoint)
            {
                user.myStat.curHealPoint = user.myStat.maxHealPoint;
            }
            item.quantity--; // 사용 후 갯수 감소
        }
    }

    public static void AttackBoostEffect(Player user, ConsumItem item)
    {
        if (item.quantity > 0)
        {
            Inventory.Instance.user.AllBuff(item.EffectPoint, item.EffectDuration, BuffType.Damage);
        }
        item.quantity--; // 사용 후 갯수 감소
    }

}
