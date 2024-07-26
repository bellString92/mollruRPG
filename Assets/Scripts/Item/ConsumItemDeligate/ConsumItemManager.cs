using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumItemManager : MonoBehaviour
{
    public ConsumItem healItem;
    public ConsumItem attackBoostItem;

    void Start()
    {
        InitializeItems();
    }

    void InitializeItems()
    {
        healItem = ScriptableObject.CreateInstance<ConsumItem>();
        healItem.itemName = "Healing Potion";
        healItem.EffectPoint = 50;
        healItem.useEffect = ConsumItemEffects.HealEffect;

        attackBoostItem = ScriptableObject.CreateInstance<ConsumItem>();
        attackBoostItem.itemName = "Attack Boost Potion";
        attackBoostItem.EffectPoint = 10; // 공격력 증가량 예시
        attackBoostItem.useEffect = ConsumItemEffects.AttackBoostEffect;
    }
}
