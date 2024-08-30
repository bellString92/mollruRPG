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
    public float EffectCoolTime;

    public float lastUseTime; // 마지막 사용 시간을 기록하는 변수

    private void OnEnable()
    {
        itemTag = "ConsumItem";
        itemType = ItemType.consumItem;
        AssignEffect();
    }
    public ConsumItem(ConsumItem original) : base(original)
    {
        this.useEffect = original.useEffect;
        effectType = original.effectType;
        EffectPoint = original.EffectPoint;
        EffectDuration = original.EffectDuration;
        EffectCoolTime = original.EffectCoolTime;

    }
    public override void Use(Player user)
    {
        if (useEffect != null && !CoolTimeManager.IsOnCooldown(itemID))
        {
            if (CanEffectivelyUse(user))
            {
                useEffect(user, this);

                // 쿨타임 시작
                CoolTimeManager.StartCooldown(itemID, EffectCoolTime);

                // 사용 후 수량 감소
                quantity--;
            }
        }
    }

    // 사용 조건을 확인하는 메서드 예시
    public bool CanEffectivelyUse(Player user)
    {
        if (effectType == EffectType.HealEffect)
        {
            if (user.myStat.curHealPoint < user.myStat.maxHealPoint)
            {
                return true;
            }
            else
            {
                Debug.Log("현재 체력이 이미 최대입니다.");
                return false;
            }
        }
        return true; // 기본적으로 사용 가능
    }

    public override void TakeOff(Player myStat){}
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
