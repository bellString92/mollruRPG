using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    public ShopSlot[] slots; // ���� ���� �迭

    public TMP_Text itemInfoTextPrefab;// ������ ������ ǥ���� TextMeshPro ������
    public ScrollRect scrollView;
    public Transform content;
    public TMP_Text infoText; // ��ũ�Ѻ信 ǥ�õ� ���� TextMeshPro

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
        // ������ ��ũ�Ѻ� ������ ��� ����
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        // ���ο� ������ ���� �ؽ�Ʈ ����
        TMP_Text newText = Instantiate(itemInfoTextPrefab, content);
        infoText = newText;

        infoText.text = $"Name: {itemInfo.itemName}\n" +
                          $"Type: {itemInfo.itemType}\n" +
                          $"Description: {itemInfo.description}\n" +
                          $"Price: {itemInfo.price}";

        // �߰����� ���� ǥ�ø� ���� switch�� ���
        switch (itemInfo.itemType)
        {
            case ItemType.weaponItem:
                DisplayWeaponInfo(itemInfo as WeaponItem);
                break;
            case ItemType.armorItem:
                DisplayArmorInfo(itemInfo as ArmorItem);
                break;
            case ItemType.acceItem:
                DisplayAccessoryInfo(itemInfo as AcceItem);
                break;
            case ItemType.consumItem:
                DisplayConsumableInfo(itemInfo as ConsumItem);
                break;
            case ItemType.materialItem:
                DisplayMaterialInfo(itemInfo as MaterialItem);
                break;
            default:
                Debug.LogWarning("Unknown ItemType: " + itemInfo.itemType);
                break;
        }

    }

    // �� ItemType�� ���� �߰� ���� ǥ�� �Լ���
    private void DisplayWeaponInfo(WeaponItem weaponInfo)
    {
        if (weaponInfo != null)
        {
            infoText.text += $"\nattackBoost: {weaponInfo.attackBoost}\n";
        }
    }

    private void DisplayArmorInfo(ArmorItem armorInfo)
    {
        if (armorInfo != null)
        {
            infoText.text += $"\nmaxHealBoost: {armorInfo.maxHealBoost}\n";
        }
    }

    private void DisplayAccessoryInfo(AcceItem acceInfo)
    {
        if (acceInfo != null)
        {
            infoText.text += $"\nattackBoost: {acceInfo.attackBoost}\n";
            infoText.text += $"\nmaxHealBoost: {acceInfo.maxHealBoost}\n";
        }
    }

    private void DisplayConsumableInfo(ConsumItem ConsumInfo)
    {
        if(ConsumInfo != null)
        {
            infoText.text += $"\nhealPoint: {ConsumInfo.healAmount}\n";
        }
    }

    private void DisplayMaterialInfo(MaterialItem MaterialInfo)
    {

    }
}
