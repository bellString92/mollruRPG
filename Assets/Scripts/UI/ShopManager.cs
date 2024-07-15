using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    public ShopSlot[] slots; // 상점 슬롯 배열
    public TextMeshProUGUI itemInfoText; // 아이템 정보를 표시할 TextMeshPro

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // 중복 생성 방지
    }

    void Start()
    {
        // 상점 초기화 함수 호출
        InitializeShop();
    }
    private void InitializeShop()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].InitializeSlot(slots[i].item); // 각 슬롯의 아이템 정보를 초기화
        }
    }
    public void OnSlotClicked(ItemKind itemInfo)
    {
        // 아이템 정보를 TextMeshPro에 표시
        if (itemInfo != null)
        {
            DisplayItemInfo(itemInfo);
        }
    }

    // 아이템 정보를 TextMeshPro에 표시하는 함수
    private void DisplayItemInfo(ItemKind itemInfo)
    {
        string infoText = $"Name: {itemInfo.itemName}\n" +
                          $"Type: {itemInfo.itemType}\n" +
                          $"Description: {itemInfo.description}\n" +
                          $"Price: {itemInfo.price}";

        // 추가적인 정보 표시를 위해 switch문 사용
        switch (itemInfo.itemType)
        {
            case ItemType.weaponItem:
                DisplayWeaponInfo(itemInfo as WeaponItem, ref infoText);
                break;
            case ItemType.armorItem:
                DisplayArmorInfo(itemInfo as ArmorItem, ref infoText);
                break;
            case ItemType.acceItem:
                DisplayAccessoryInfo(itemInfo as AcceItem, ref infoText);
                break;
            case ItemType.consumItem:
                DisplayConsumableInfo(itemInfo as ConsumItem, ref infoText);
                break;
            case ItemType.materialItem:
                DisplayMaterialInfo(itemInfo as MaterialItem, ref infoText);
                break;
            default:
                Debug.LogWarning("Unknown ItemType: " + itemInfo.itemType);
                break;
        }

        itemInfoText.text = infoText;
    }

    // 각 ItemType에 따른 추가 정보 표시 함수들
    private void DisplayWeaponInfo(WeaponItem weaponInfo, ref string infoText)
    {
        if (weaponInfo != null)
        {
            infoText += $"\nattackBoost: {weaponInfo.attackBoost}\n";
        }
    }

    private void DisplayArmorInfo(ArmorItem armorInfo, ref string infoText)
    {
        if (armorInfo != null)
        {
            infoText += $"\nmaxHealBoost: {armorInfo.maxHealBoost}\n";
        }
    }

    private void DisplayAccessoryInfo(AcceItem acceInfo, ref string infoText)
    {
        if (acceInfo != null)
        {
            infoText += $"\nattackBoost: {acceInfo.attackBoost}\n";
            infoText += $"\nmaxHealBoost: {acceInfo.maxHealBoost}\n";
        }
    }

    private void DisplayConsumableInfo(ConsumItem ConsumInfo, ref string infoText)
    {
        if(ConsumInfo != null)
        {
            infoText += $"\nhealPoint: {ConsumInfo.healAmount}\n";
        }
    }

    private void DisplayMaterialInfo(MaterialItem MaterialInfo, ref string infoText)
    {

    }
}
