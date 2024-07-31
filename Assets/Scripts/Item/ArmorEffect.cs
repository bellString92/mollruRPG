using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArmorEffect
{
    public static void ApplyEffects(Player user, ArmorItem item)
    {
        foreach (var effect in item.effectList)
        {
            switch (effect.effectType)
            {
                case ArmorEffectType.AttackBoost:
                    user.myStat.maxHealPoint += effect.effectValue;
                    break;
                case ArmorEffectType.CritChanceBoost:
                    user.myStat.CriticalProbability += effect.effectValue;
                    break;
                case ArmorEffectType.CritDamageBoost:
                    user.myStat.CriticalDamage += effect.effectValue;
                    break;
                case ArmorEffectType.SpeedBoost:
                    user.myStat.moveSpeed += effect.effectValue;
                    break;
                    // 추가 효과 로직
            }
        }
    }

    public static void RemoveEffects(Player user, ArmorItem item)
    {
        foreach (var effect in item.effectList)
        {
            switch (effect.effectType)
            {
                case ArmorEffectType.AttackBoost:
                    user.myStat.maxHealPoint -= effect.effectValue;
                    break;
                case ArmorEffectType.CritChanceBoost:
                    user.myStat.CriticalProbability -= effect.effectValue;
                    break;
                case ArmorEffectType.CritDamageBoost:
                    user.myStat.CriticalDamage -= effect.effectValue;
                    break;
                case ArmorEffectType.SpeedBoost:
                    user.myStat.moveSpeed -= effect.effectValue;
                    break;
                    // 추가 효과 로직
            }
        }
    }
}
