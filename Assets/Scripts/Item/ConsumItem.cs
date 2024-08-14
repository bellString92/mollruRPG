using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

 public delegate void UseEffect(Player user, ConsumItem item);
public enum EffectType
{
    HealEffect,
    AttackBoostEffect,
    SpeedBoostEffect
    // 새로운 효과 추가
}

[CreateAssetMenu(fileName = "New ConsumItem", menuName = "Items/ConsumItem")]// Asset/create창에서 아이템을 생성시키게 할수있는 코드
public class ConsumItem : ItemKind
{
    public EffectType effectType;
    public UseEffect useEffect;

    public float EffectPoint;
    public float EffectDuration;


    private void OnEnable()
    {
        itemTag = "ConsumItem";
        itemType = ItemType.consumItem;
        maxStack = 99;
        AssignEffect();
    }
    public ConsumItem(ConsumItem original) : base(original)
    {
        this.useEffect = original.useEffect;
        EffectPoint = original.EffectPoint;
        EffectDuration = original.EffectDuration;
    }
    public override void Use(Player user) //사용시 player의 능력치에 영향을 주는 코드
    {
        if (useEffect != null)
        {
            useEffect(user, this);
        }
    }
    public override void TakeOff(Player myStat)
    {
    
    }
    public void AssignEffect()
    {
        switch (effectType)
        {
            case EffectType.HealEffect:
                useEffect = ConsumItemEffects.HealEffect;
                break;
            case EffectType.AttackBoostEffect:
                useEffect = ConsumItemEffects.AttackBoostEffect;
                break;
            case EffectType.SpeedBoostEffect:
                useEffect = ConsumItemEffects.SpeedBoostEffect;
                break;
            // 새로운 효과 할당
            default:
                useEffect = null;
                break;
        }
    }
}
