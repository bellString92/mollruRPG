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

    public ItemKind curItem = null;// ���� �������� ������ ������ ������ ����
    public GameObject marterialObject;

    private InventorySlot lastClickedSlot = null;
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
            curItem = itemInfo;
        }
        
    }

    public void OnAddNewItemInInventory()
    {
        ItemKind copiedItem = Instantiate(curItem); // ���� ������ �����͸� �����ϱ����� ��������


        // AssemblyManager�� CreateItem ȣ���Ͽ� ������ ����
        if (curItem != null)
        {
            //if (copiedItem.itemType == ItemType.consumItem || copiedItem.itemType == ItemType.materialItem)
            //{
            //    // UI�� ���� quantity ���� �����ϵ��� ����
            //    UIManager.Instance.OpenQuantityUI(copiedItem, () =>
            //    {
            //        // ����ڰ� ��ư�� ������ ȣ��Ǵ� �ݹ�
            //        Inventory.Instance.CreateItem(copiedItem, marterialObject);
            //    });
            //}
            //else
            /*{*/
                // ������ Ÿ���� consumItem �Ǵ� materialItem�� �ƴ� ��� �ٷ� �κ��丮�� �߰�
                Inventory.Instance.CreateItem(copiedItem, marterialObject);
            /*}*/
        }
    }

    public void SetDestroySlotItem(InventorySlot slot) // �Ǹ� ��ư Ŭ���� ��� ������ ���޹޴� ��
    {
        lastClickedSlot = slot;
    }
    public void OnDestroyInventorySlotItem() // �Ǹ� ��ư�� ������ ȣ��
    {
        if (lastClickedSlot != null)
        {
            lastClickedSlot.DestroyChild();
            lastClickedSlot = null;
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

        newText.text = $"Name: {itemInfo.itemName}\n" +
                          $"Type: {itemInfo.itemType}\n" +
                          $"Description: {itemInfo.description}\n" +
                          $"Price: {itemInfo.price}";

        // �߰����� ���� ǥ�ø� ���� switch�� ���
        switch (itemInfo.itemType)
        {
            case ItemType.weaponItem:
                DisplayWeaponInfo(itemInfo as WeaponItem, newText);
                break;
            case ItemType.armorItem:
                DisplayArmorInfo(itemInfo as ArmorItem, newText);
                break;
            case ItemType.acceItem:
                DisplayAccessoryInfo(itemInfo as AcceItem, newText);
                break;
            case ItemType.consumItem:
                DisplayConsumableInfo(itemInfo as ConsumItem, newText);
                break;
            case ItemType.materialItem:
                DisplayMaterialInfo(itemInfo as MaterialItem, newText);
                break;
            default:
                Debug.LogWarning("Unknown ItemType: " + itemInfo.itemType);
                break;
        }

    }

    // �� ItemType�� ���� �߰� ���� ǥ�� �Լ���
    private void DisplayWeaponInfo(WeaponItem weaponInfo, TMP_Text infoText)
    {
        if (weaponInfo != null)
        {
            infoText.text += $"\nattackBoost: {weaponInfo.attackBoost}\n";
        }
    }

    private void DisplayArmorInfo(ArmorItem armorInfo, TMP_Text infoText)
    {
        if (armorInfo != null)
        {
            infoText.text += $"\nmaxHealBoost: {armorInfo.maxHealBoost}\n";
        }
    }

    private void DisplayAccessoryInfo(AcceItem acceInfo, TMP_Text infoText)
    {
        if (acceInfo != null)
        {
            infoText.text += $"\nattackBoost: {acceInfo.attackBoost}\n";
            infoText.text += $"\nmaxHealBoost: {acceInfo.maxHealBoost}\n";
        }
    }

    private void DisplayConsumableInfo(ConsumItem ConsumInfo, TMP_Text infoText)
    {
        if(ConsumInfo != null)
        {
            infoText.text += $"\nhealPoint: {ConsumInfo.healAmount}\n";
        }
    }

    private void DisplayMaterialInfo(MaterialItem MaterialInfo, TMP_Text infoText)
    {

    }
}
