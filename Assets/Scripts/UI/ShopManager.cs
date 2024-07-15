using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    public ShopSlot[] slots; // ���� ���� �迭
    public TextMeshProUGUI itemInfoText; // ������ ������ ǥ���� TextMeshPro

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // �ߺ� ���� ����
    }

    void Start()
    {
        // ���� �ʱ�ȭ �Լ� ȣ��
        InitializeShop();
    }
    private void InitializeShop()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].InitializeSlot(slots[i].item); // �� ������ ������ ������ �ʱ�ȭ
        }
    }
    public void OnSlotClicked(ItemKind itemInfo)
    {
        // ������ ������ TextMeshPro�� ǥ��
        if (itemInfo != null)
        {
            DisplayItemInfo(itemInfo);
        }
    }

    // ������ ������ TextMeshPro�� ǥ���ϴ� �Լ�
    private void DisplayItemInfo(ItemKind itemInfo)
    {
        string infoText = $"Name: {itemInfo.itemName}\n" +
                          $"Type: {itemInfo.itemType}\n" +
                          $"Description: {itemInfo.description}\n" +
                          $"Price: {itemInfo.price}";

        // �߰����� ���� ǥ�ø� ���� switch�� ���
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

    // �� ItemType�� ���� �߰� ���� ǥ�� �Լ���
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
