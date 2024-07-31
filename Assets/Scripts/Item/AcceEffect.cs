using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AcceEffect 
{
    public static void ApplyEffects(Player user, AcceItem item)
    {
        foreach (var effect in item.effectList)
        {
            switch (effect.effectType)
            {
                case AcceEffectType.AttackBoost:
                    user.myStat.AttackPoint += effect.effectValue;
                    break;
                case AcceEffectType.MaxHealthBoost:
                    user.myStat.maxHealPoint += effect.effectValue;
                    break;
                case AcceEffectType.CritChanceBoost:
                    user.myStat.CriticalProbability += effect.effectValue;
                    break;
                case AcceEffectType.CritDamageBoost:
                    user.myStat.CriticalDamage += effect.effectValue;
                    break;
                case AcceEffectType.SpeedBoost:
                    user.myStat.moveSpeed += effect.effectValue;
                    break;
                    // 추가 효과 로직
            }
        }
    }

    public static void RemoveEffects(Player user, AcceItem item)
    {
        foreach (var effect in item.effectList)
        {
            switch (effect.effectType)
            {
                case AcceEffectType.AttackBoost:
                    user.myStat.AttackPoint -= effect.effectValue;
                    break;
                case AcceEffectType.MaxHealthBoost:
                    user.myStat.maxHealPoint -= effect.effectValue;
                    break;
                case AcceEffectType.CritChanceBoost:
                    user.myStat.CriticalProbability -= effect.effectValue;
                    break;
                case AcceEffectType.CritDamageBoost:
                    user.myStat.CriticalDamage -= effect.effectValue;
                    break;
                case AcceEffectType.SpeedBoost:
                    user.myStat.moveSpeed -= effect.effectValue;
                    break;
                    // 추가 효과 로직
            }
        }
    }
}
