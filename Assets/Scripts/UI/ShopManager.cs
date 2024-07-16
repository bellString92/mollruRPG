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
    public ShopSlot[] slots; // 상점 슬롯 배열

    public TMP_Text itemInfoTextPrefab;// 아이템 정보를 표시할 TextMeshPro 프리팹
    public ScrollRect scrollView;
    public Transform content;

    public ItemKind curItem = null;// 현재 선택중인 아이템 정보를 저장할 변수
    public GameObject marterialObject;

    private InventorySlot lastClickedSlot = null;
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
            curItem = itemInfo;
        }
        
    }

    public void OnAddNewItemInInventory()
    {
        // AssemblyManager의 CreateItem 호출하여 아이템 생성
        if (curItem != null)
        {
            Inventory.Instance.CreateItem(curItem, marterialObject);
        }
    }

    public void SetDestroySlotItem(InventorySlot slot) // 판ㅁ매 버튼 클릭시 비울 슬롯을 전달받는 곳
    {
        lastClickedSlot = slot;
    }
    public void OnDestroyInventorySlotItem() // 판매 버튼을 누르면 호출
    {
        if (lastClickedSlot != null)
        {
            lastClickedSlot.DestroyChild();
            lastClickedSlot = null;
        }
    }



    // 아이템 정보를 TextMeshPro에 표시하는 함수
    private void DisplayItemInfo(ItemKind itemInfo)
    {
        // 기존의 스크롤뷰 콘텐츠 모두 제거
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        // 새로운 아이템 정보 텍스트 생성
        TMP_Text newText = Instantiate(itemInfoTextPrefab, content);

        newText.text = $"Name: {itemInfo.itemName}\n" +
                          $"Type: {itemInfo.itemType}\n" +
                          $"Description: {itemInfo.description}\n" +
                          $"Price: {itemInfo.price}";

        // 추가적인 정보 표시를 위해 switch문 사용
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

    // 각 ItemType에 따른 추가 정보 표시 함수들
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
