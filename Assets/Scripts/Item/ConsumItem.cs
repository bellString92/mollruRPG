using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public delegate void UseEffect(BattleStat myStat, ConsumItem item);
public enum EffectType
{
    HealEffect,
    AttackBoostEffect,
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
        AssignEffect();
    }
    public override void Use(BattleStat myStat) //사용시 player의 능력치에 영향을 주는 코드
    {
        if (useEffect != null)
        {
            useEffect(myStat, this);
        }
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
            // 새로운 효과 할당
            default:
                useEffect = null;
                break;
        }
    }
}
