using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New ArmorItem", menuName = "Items/ArmorItem")]// Asset/create창에서 아이템을 생성시키게 할수있는 코드
public class ArmorItem : ItemKind
{
    public float maxHealBoost;
    public List<UpgradeRequirement> armorUpgradeRequirements;   // 갑옷 강화 요구사항 리스트

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


    // 갑옷 타입에 맞는 재료 요구사항 반환 메서드
    public List<MaterialRequirement> GetMaterialRequirementsForLevel(int level)
    {
        if (level >= GetMaxKaiLevel(rarity)+1)
        {
            return null; // 최대 강화레벨에 도달했거나 초과하면 null 반환
        }
        List<MaterialRequirement> materialRequirements = new List<MaterialRequirement>();

        foreach (var requirement in armorUpgradeRequirements)
        {
            if (level >= requirement.startLevel && level <= requirement.endLevel)
            {
                int levelInRange = level - requirement.startLevel + 1;

                foreach (var materialIncrement in requirement.materialIncrements)
                {
                    var existingRequirement = materialRequirements.FirstOrDefault(m => m.requiredItem == materialIncrement.material);
                    if (existingRequirement != null)
                    {
                        existingRequirement.quantity += materialIncrement.increment * levelInRange;
                    }
                    else
                    {
                        materialRequirements.Add(new MaterialRequirement
                        {
                            requiredItem = materialIncrement.material,
                            quantity = materialIncrement.increment * levelInRange
                        });
                    }
                }
                break;
            }
            else if (level > requirement.endLevel)
            {
                int levelsInThisRange = requirement.endLevel - requirement.startLevel + 1;

                foreach (var materialIncrement in requirement.materialIncrements)
                {
                    var existingRequirement = materialRequirements.FirstOrDefault(m => m.requiredItem == materialIncrement.material);
                    if (existingRequirement != null)
                    {
                        existingRequirement.quantity += materialIncrement.increment * levelsInThisRange;
                    }
                    else
                    {
                        materialRequirements.Add(new MaterialRequirement
                        {
                            requiredItem = materialIncrement.material,
                            quantity = materialIncrement.increment * levelsInThisRange
                        });
                    }
                }
            }
        }

        return materialRequirements;
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
