using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ForgeUI : MonoBehaviour
{
    public static ForgeUI Instance;
    public TMP_Text itemInfoTextPrefab; // 아이템 정보를 표시할 TextMeshPro 프리팹
    public Transform scrollViewContent; // Scroll View의 Content Transform
    public TextMeshProUGUI LuckPercent; // 확률을 보여줄 TextMeshPro
    public MaterialRequirementDisplay materialRequirementDisplay; // 요구 재료를 표시할 영역 

    public ForgeSlot forgeSlot; // 포지 슬롯 참조

    private void Awake()
    {
        Instance = this;
    }

    // 아이템 정보를 표시하는 메서드
    public void DisplayItemInfo(SaveItemInfo saveItemInfo)
    {
        // 현재 아이템의 강화 성공 확률 표시
        UpdateLuckPercent(saveItemInfo.itemKind);

        // 기존의 스크롤뷰 콘텐츠 모두 제거
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // SaveItemInfo에서 ItemKind 정보 가져오기
        ItemKind itemInfo = saveItemInfo.itemKind;

        // 새로운 아이템 정보 텍스트 생성
        TMP_Text newText = Instantiate(itemInfoTextPrefab, scrollViewContent);

        materialRequirementDisplay.DisplayMaterialRequirements(itemInfo);

        if (itemInfo.kaiLevel != 0)
        {
            newText.text = $"Name: {itemInfo.itemName} +{itemInfo.kaiLevel}\n";
        }
        else
        {
            newText.text = $"Name: {itemInfo.itemName} \n";
        }
        newText.text += $"Description: {itemInfo.description}\n";


        // ItemType에 따른 추가 정보 표시
        switch (itemInfo.itemType)
        {
            case ItemType.weaponItem:
                DisplayWeaponInfo(itemInfo as WeaponItem, newText);
                break;
            case ItemType.armorItem:
                DisplayArmorInfo(itemInfo as ArmorItem, newText);
                break;
            default:
                Debug.LogWarning("Unknown ItemType: " + itemInfo.itemType);
                break;
        }
    }

    // 무기 아이템 정보 표시
    void DisplayWeaponInfo(WeaponItem weaponInfo, TMP_Text infoText)
    {
        
        if (weaponInfo != null)
        {
            int maxKaiLevel = weaponInfo.GetMaxKaiLevel(weaponInfo.rarity);
            float[] incrementValues = weaponInfo.GetIncrementValuesByRarity(weaponInfo.rarity);

            float currentBoost = weaponInfo.CalculateEffectiveAttackBoost();
            float nextBoost = weaponInfo.CalculateEffectiveAttackBoost(weaponInfo.kaiLevel + 1);

            if (weaponInfo.kaiLevel < maxKaiLevel)
            {
                infoText.text += $"Attack Boost: {currentBoost} => {nextBoost}\n";
            }
            else
            {
                infoText.text += $"Attack Boost: {currentBoost} (최고 강화 등급입니다)\n";
            }
        }
    }

    // 방어구 아이템 정보 표시
    void DisplayArmorInfo(ArmorItem armorInfo, TMP_Text infoText)
    {
        if (armorInfo != null)
        {
            int maxKaiLevel = armorInfo.GetMaxKaiLevel(armorInfo.rarity);
            float[] incrementValues = armorInfo.GetIncrementValuesByRarity(armorInfo.rarity);

            float currentBoost = armorInfo.CalculateEffectiveMaxHealBoost();
            float nextBoost = armorInfo.CalculateEffectiveMaxHealBoost(armorInfo.kaiLevel + 1);

            if (armorInfo.kaiLevel < maxKaiLevel)
            {
                infoText.text += $"Max Heal Boost: {currentBoost} => {nextBoost}\n";
            }
            else
            {
                infoText.text += $"Max Heal Boost: {currentBoost} (최고 강화 등급입니다)\n";
            }
        }
    }
    public void IncreaseKaiLevel(SaveItemInfo saveItemInfo)
    {
        if (saveItemInfo != null)
        {
            ItemKind itemInfo = saveItemInfo.itemKind;
            List<MaterialRequirement> materialRequirements = null; // 요구 재료 리스트 불러오기

            if (itemInfo is WeaponItem weaponItem) // 아이템 타입 검사후 맞는 리스트 가져오기
            {
                materialRequirements = weaponItem.GetMaterialRequirementsForLevel(itemInfo.kaiLevel + 1);
            }
            else if (itemInfo is ArmorItem armorItem)
            {
                materialRequirements = armorItem.GetMaterialRequirementsForLevel(itemInfo.kaiLevel + 1);
            }


            if (materialRequirements != null)
            {
                bool hasAllMaterials = true; // 검사하기전 true로 초기화
                foreach (var material in materialRequirements)// 인벤토리에서 요구 재료의 종류와 수량이 충분한지 검사
                {
                    if (!Inventory.Instance.HasItem(material.requiredItem, material.quantity))
                    {
                        hasAllMaterials = false; // 충분하지 않으면 bool 값 변화
                        break;
                    }
                }

                // 모든 아이템이 존재하고, 최대 강화 수치 초과하지 않도록 체크
                if (hasAllMaterials && itemInfo.kaiLevel < itemInfo.GetMaxKaiLevel(itemInfo.rarity))
                {
                    float successRate = itemInfo.GetUpgradeSuccessRate();

                    if (Random.value <= successRate) // Random.value는 0.0과 1.0 사이의 무작위 값을 반환
                    {
                        itemInfo.KaiLevel += 1; // 강화 레벨 증가
                        Debug.Log("강화 성공!");
                    }
                    else
                    {
                        Debug.Log("강화 실패");
                    }
                    foreach (var material in materialRequirements)
                    {
                        Inventory.Instance.RemoveItem(material.requiredItem, material.quantity);
                    }
                    DisplayItemInfo(saveItemInfo); // 아이템 정보 갱신
                }
                else if(hasAllMaterials == false)
                {
                    Debug.Log("재료가 부족합니다.");
                }
                else
                {
                    Debug.Log("최고 강화 등급입니다.");
                }
            }
        }
    }

    // 확률을 퍼센트로 변환하여 LuckPercent에 표시하는 메서드
    private void UpdateLuckPercent(ItemKind itemInfo)
    {
        float successRate = itemInfo.GetUpgradeSuccessRate();
        LuckPercent.text = $"성공 확률 : {successRate * 100}%";
    }

    private void OnDestroy()
    {
        if (forgeSlot != null && forgeSlot.myChild != null)
        {
            // 창 종료 시점에 인벤토리로 아이템을 보내는 로직 구현
            SaveItemInfo saveItemInfo = forgeSlot.myChild.GetComponent<SaveItemInfo>();
            if (saveItemInfo != null)
            {               
                Inventory.Instance.CreateItem(saveItemInfo.itemKind, forgeSlot.myChild);
                forgeSlot.myChild = null;
            }
        }
    }
}