using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public class ConsumItemEffects : MonoBehaviour
{
    public static void HealEffect(Player user, ConsumItem item)
    {
        user.myStat.curHealPoint += item.EffectPoint;
        if (user.myStat.curHealPoint > user.myStat.maxHealPoint)
        {
            user.myStat.curHealPoint = user.myStat.maxHealPoint;
        }
        user.TakeDamage(0);
    }

    public static void AttackBoostEffect(Player user, ConsumItem item)
    {
        Inventory.Instance.user.AllBuff(item.EffectPoint, item.EffectDuration, BuffType.Damage, BuffSource.ConsumableItem);
    }

    public static void SpeedBoostEffect(Player user, ConsumItem item)
    {
        Inventory.Instance.user.AllBuff(item.EffectPoint, item.EffectDuration, BuffType.MoveSpeed, BuffSource.ConsumableItem);
    }
}
