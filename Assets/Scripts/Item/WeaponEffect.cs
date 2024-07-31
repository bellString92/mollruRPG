using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeaponEffect
{ 
    public static void ApplyEffects(Player user, WeaponItem item)
    {
        foreach (var effect in item.effectList)
        {
            switch (effect.effectType)
            {
                case WeaponEffectType.MaxHealthBoost:
                    user.myStat.maxHealPoint += effect.effectValue;
                    break;
                case WeaponEffectType.CritChanceBoost:
                    user.myStat.CriticalProbability += effect.effectValue;
                    break;
                case WeaponEffectType.CritDamageBoost:
                    user.myStat.CriticalDamage += effect.effectValue;
                    break;
                case WeaponEffectType.SpeedBoost:
                    user.myStat.moveSpeed += effect.effectValue;
                    break;
                    // 추가 효과 로직
            }
        }
    }

    public static void RemoveEffects(Player user, WeaponItem item)
    {
        foreach (var effect in item.effectList)
        {
            switch (effect.effectType)
            {
                case WeaponEffectType.MaxHealthBoost:
                    user.myStat.maxHealPoint -= effect.effectValue;
                    break;
                case WeaponEffectType.CritChanceBoost:
                    user.myStat.CriticalProbability -= effect.effectValue;
                    break;
                case WeaponEffectType.CritDamageBoost:
                    user.myStat.CriticalDamage -= effect.effectValue;
                    break;
                case WeaponEffectType.SpeedBoost:
                    user.myStat.moveSpeed -= effect.effectValue;
                    break;
                    // 추가 효과 로직
            }
        }
    }
}
