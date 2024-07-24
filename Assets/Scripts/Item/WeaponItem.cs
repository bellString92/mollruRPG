using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponItem", menuName = "Items/WeaponItem")]// Asset/create창에서 아이템을 생성시키게 할수있는 코드
public class WeaponItem : ItemKind
{
    public float attackBoost;
    public List<UpgradeRequirement> weaponUpgradeRequirements;  // 무기 강화 요구사항 리스트

    private void OnEnable()
    {
        itemTag = "WeaponItem";
        itemType = ItemType.weaponItem;
        maxStack = 1;
    }
    public WeaponItem(WeaponItem original) : base(original)
    {
        attackBoost = original.attackBoost;
    }

    public override void Use(BattleStat myStat) //사용시 player의 능력치에 영향을 주는 코드
    {
        float effectiveAttackBoost = CalculateEffectiveAttackBoost();
        myStat.AttackPoint += attackBoost;
    }


    public List<MaterialRequirement> GetMaterialRequirementsForLevel(int level)
    {
        return weaponUpgradeRequirements.FirstOrDefault(req => req.kaiLevel == level)?.materialRequirements;
    }

    // kaiLevel에 따른 attackBoost 증가 수치 계산
    public float CalculateEffectiveAttackBoost(int level = -1)
    {
        if (level == -1)
        {
            level = kaiLevel;
        }

        float boost = attackBoost;
        float[] incrementValues = GetIncrementValuesByRarity(rarity);

        // 레벨 구간에 따른 증가 비율 적용
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
