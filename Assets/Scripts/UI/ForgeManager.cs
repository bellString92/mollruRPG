using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ForgeUI : MonoBehaviour
{
    public TMP_Text itemInfoTextPrefab; // 아이템 정보를 표시할 TextMeshPro 프리팹
    public Transform scrollViewContent; // Scroll View의 Content Transform

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

        newText.text = $"Name: {itemInfo.itemName}\n" +
                       $"Type: {itemInfo.itemType}\n" +
                       $"Description: {itemInfo.description}\n" +
                       $"Price: {itemInfo.price}\n";

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
            infoText.text += $"Attack Boost: {weaponInfo.attackBoost}\n";
        }
    }

    // 방어구 아이템 정보 표시
    void DisplayArmorInfo(ArmorItem armorInfo, TMP_Text infoText)
    {
        if (armorInfo != null)
        {
            infoText.text += $"Max Heal Boost: {armorInfo.maxHealBoost}\n";
        }
    }
}