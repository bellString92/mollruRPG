using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ArmorItem", menuName = "Items/ArmorItem")]// Asset/create창에서 아이템을 생성시키게 할수있는 코드
public class ArmorItem : ItemKind
{
    public float maxHealBoost;

    private void OnEnable()
    {
        itemTag = "ArmorItem";
        itemType = ItemType.armorItem;
        maxStack = 1;
    }
    public ArmorItem(ArmorItem original) : base(original)
    {
        maxHealBoost = original.maxHealBoost;
    }

    public override void Use(BattleStat myStat) //사용시 player의 능력치에 영향을 주는 코드
    {
        float effectiveMaxHealBoost = CalculateEffectiveMaxHealBoost();
        myStat.maxHealPoint += maxHealBoost;
        myStat.curHealPoint += maxHealBoost;
    }

    // kaiLevel에 따른 최대 회복 증가 수치 계산
    public float CalculateEffectiveMaxHealBoost(int level = -1)
    {
        if (level == -1)
        {
            level = kaiLevel;
        }

        float boost = maxHealBoost;
        float[] incrementValues = GetIncrementValuesByRarity(rarity);

        if (level >= 1 && level <= 5)
        {
            boost *= Mathf.Pow(incrementValues[0], level);
        }
        else if (level >= 6 && level <= 10)
        {
            boost *= Mathf.Pow(incrementValues[1], level - 5);
        }
        else if (level >= 11 && level <= 15)
        {
            boost *= Mathf.Pow(incrementValues[2], level - 10);
        }
        else if (level >= 16 && level <= 20)
        {
            boost *= Mathf.Pow(incrementValues[3], level - 15);
        }

        return boost;
    }
}
