using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ForgeUI : MonoBehaviour
{
    public static ForgeUI Instance;
    public TMP_Text itemInfoTextPrefab; // 아이템 정보를 표시할 TextMeshPro 프리팹
    public Transform scrollViewContent; // Scroll View의 Content Transform

    private void Awake()
    {
        Instance = this;
    }

    // 아이템 정보를 표시하는 메서드
    public void DisplayItemInfo(SaveItemInfo saveItemInfo)
    {
        // 기존의 스크롤뷰 콘텐츠 모두 제거
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // SaveItemInfo에서 ItemKind 정보 가져오기
        ItemKind itemInfo = saveItemInfo.itemKind;

        // 새로운 아이템 정보 텍스트 생성
        TMP_Text newText = Instantiate(itemInfoTextPrefab, scrollViewContent);

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

            // 최대 강화 수치 초과하지 않도록 체크
            if (itemInfo.kaiLevel < itemInfo.GetMaxKaiLevel(itemInfo.rarity))
            {
                itemInfo.KaiLevel += 1; // 카이 레벨 증가
                DisplayItemInfo(saveItemInfo); // 아이템 정보 갱신
            }
            else
            {
                Debug.Log("최고 강화 등급입니다.");
            }
        }
    }
}