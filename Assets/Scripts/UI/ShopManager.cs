using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine.EventSystems;

using static UnityEditor.Progress;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    public ShopSlot[] slots; // 상점 슬롯 배열

    public TMP_Text itemInfoTextPrefab;// 아이템 정보를 표시할 TextMeshPro 프리팹
    public ScrollRect scrollView;
    public Transform content;

    public ItemKind curItem = null;// 현재 선택중인 아이템 정보를 저장할 변수
    public ShopSlot curSlot = null;
    public ShopSlot beforSlot = null;

    public ItemKind curItemforSell = null;
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

    public void ShowInfo(ItemKind iteminfo, ShopSlot setSlot)
    {
        if (curSlot != setSlot)
        { 
            beforSlot = curSlot;
            curSlot = setSlot;
            curSlot.SelectSlotCheck();
            beforSlot?.UnSelectSlotCheck();
        }
        DisplayItemInfo(iteminfo);
        curItem= iteminfo;
    }


    // 구매
    public void OnAddNewItemInInventory()
    {
        ItemKind copiedItem = Instantiate(curItem); // 원본 아이템 데이터를 보존하기위한 정보 복사

        if (curItem != null)
        {
            if (copiedItem.itemType == ItemType.consumItem || copiedItem.itemType == ItemType.materialItem)
            {
                // UI를 통해 quantity 수정 가능하도록 설정
                UIManager.Instance.OpenQuantityUI(copiedItem, () =>
                {
                    // 사용자가 버튼을 누르면 호출되는 콜백
                    FinalizePurchase(copiedItem);
                });
            }
            else
            {
                // 아이템 타입이 consumItem 또는 materialItem이 아닌 경우 바로 인벤토리에 추가
                FinalizePurchase(copiedItem);
            }
        }
    }

    private void FinalizePurchase(ItemKind copiedItem)
    {
        int totalCost = copiedItem.price * copiedItem.quantity;
        Inventory inven = Inventory.Instance;
        // 사용자의 소지 금액이 충분한지 확인
        if (inven.user.myStat.myGold >= totalCost)
        {
            // 금액이 충분한 경우 아이템을 인벤토리에 추가
            inven.CreateItem(copiedItem, marterialObject);

            // 소지 금액에서 아이템 가격을 차감
            inven.user.myStat.myGold -= totalCost;
        }
        else
        {
            // 금액이 부족한 경우 경고 메시지 출력 또는 다른 로직 수행
            UIManager.Instance.ShowOkbuttonUI(inven.NoEmptySlotPopup, OkBoxType.NotEnoughGold);
        }
    }

    public void SetDestroySlotItem(InventorySlot slot) // 판매 버튼 클릭시 비울 슬롯을 전달받는 곳
    {
        lastClickedSlot = slot;

        // 슬롯의 자식에서 ItemKind 정보를 가져오기
        if (lastClickedSlot != null && lastClickedSlot.myChild != null)
        {
            SaveItemInfo saveItemInfo = lastClickedSlot.myChild.GetComponent<SaveItemInfo>();
            if (saveItemInfo != null)
            {
                curItemforSell = saveItemInfo.item; // 현재 선택된 아이템 정보를 저장
            }
        }
        else
        {
            curItemforSell = null;
        }
    }


    // 판매
    public void OnDestroyInventorySlotItem() // 판매 버튼을 누르면 호출
    {
        if (lastClickedSlot != null)
        {
            if (curItemforSell.itemType == ItemType.consumItem || curItemforSell.itemType == ItemType.materialItem)
            {
                // UI를 통해 quantity 수정 가능하도록 설정
                UIManager.Instance.OpenSellQuantityCheckUI(curItemforSell, () =>
                {
                    // 사용자가 버튼을 누르면 호출되는 콜백
                    // UI에서 centerText의 값을 읽어와서 quantity를 감소시킴
                    int quantityToDeduct = curItemforSell.quantity; // 기본적으로 현재 quantity를 사용
                    if (int.TryParse(UIManager.Instance.GetSellQuantityText(), out int enteredQuantity))
                    {
                        quantityToDeduct = Mathf.Clamp(enteredQuantity, 0, curItemforSell.quantity); // 유효한 범위로 클램프
                    }

                    curItemforSell.quantity -= quantityToDeduct; // quantity 감소
                    if (curItemforSell.quantity <= 0)
                    {
                        // quantity가 0 이하로 떨어지면 아이템을 제거
                        lastClickedSlot.DestroyChild();
                    }

                    lastClickedSlot = null;
                });
            }
            else
            {
                lastClickedSlot.DestroyChild();
                lastClickedSlot = null;
            }
        }
    }

    // 아이템 정보 상점 표시

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

        Color rareityColor;
        string rarityColorHex = "";
        switch (itemInfo.rarity)
        {
            case Rarity.Common:
                rareityColor = Color.white;
                rarityColorHex = ColorUtility.ToHtmlStringRGB(rareityColor);
                break;
            case Rarity.Uncommon:
                rareityColor = new Color(0.1f, 0.8f, 0.1f);
                rarityColorHex = ColorUtility.ToHtmlStringRGB(rareityColor);
                break;
            case Rarity.Rare:
                rareityColor = new Color(0.3f, 0.3f, 1f);
                rarityColorHex = ColorUtility.ToHtmlStringRGB(rareityColor);
                break;
            case Rarity.Epic:
                rareityColor = new Color(1, 0, 1);
                rarityColorHex = ColorUtility.ToHtmlStringRGB(rareityColor);
                break;
            case Rarity.Legendary:
                rareityColor = new Color(1, 1, 0);
                rarityColorHex = ColorUtility.ToHtmlStringRGB(rareityColor);
                break;
        }

        newText.text = $" <b>[ {itemInfo.itemName} ]</b>\n" +
                          $"등급 : <b><color=#{rarityColorHex}>{itemInfo.rarity}</color></b>\n" +
                          $"<b><color=yellow>가격 : {itemInfo.price}</color></b>\n" +
                          $"분류 : {itemInfo.itemType}\n";


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
        newText.text += $"[ {itemInfo.description} ]\n";

    }

    // 각 ItemType에 따른 추가 정보 표시 함수들
    private void DisplayWeaponInfo(WeaponItem weaponInfo, TMP_Text infoText)
    {
        if (weaponInfo != null)
        {
            // 기본적으로 모든 무기가 가지는 공격력 보너스 표시
            infoText.text += $"<b>무기 공격력 : {weaponInfo.attackBoost}</b>\n";

            // 효과 목록을 순회하여 설정된 효과만 표시
            foreach (var effect in weaponInfo.effectList)
            {
                switch (effect.effectType)
                {
                    case WeaponEffectType.MaxHealthBoost:
                        infoText.text += $"<b>추가 체력 : {effect.effectValue}</b>\n";
                        break;
                    case WeaponEffectType.CritChanceBoost:
                        infoText.text += $"<b>치명타 확률 : {effect.effectValue}</b>\n";
                        break;
                    case WeaponEffectType.CritDamageBoost:
                        infoText.text += $"<b>치명타 대미지 : {effect.effectValue}</b>\n";
                        break;
                    case WeaponEffectType.SpeedBoost:
                        infoText.text += $"<b>추가 속도 : {effect.effectValue}</b>\n";
                        break;
                    // 추가적인 효과 
                    default:
                        infoText.text += $"<b>Unknown Effect Type: {effect.effectType}</b>\n";
                        break;
                }
            }
        }
    }

    private void DisplayArmorInfo(ArmorItem armorInfo, TMP_Text infoText)
    {
        if (armorInfo != null)
        {
            // 방어구 부위 정보 표시
            infoText.text += $"부위: {armorInfo.armorType}\n";
            // 기본적으로 모든 무기가 가지는 공격력 보너스 표시
            infoText.text += $"<b>방어구 체력 : {armorInfo.maxHealBoost}</b>\n";

            // 효과 목록을 순회하여 설정된 효과만 표시
            foreach (var effect in armorInfo.effectList)
            {
                switch (effect.effectType)
                {
                    case ArmorEffectType.AttackBoost:
                        infoText.text += $"<b>추가 공격력: {effect.effectValue}</b>\n";
                        break;
                    case ArmorEffectType.CritChanceBoost:
                        infoText.text += $"<b>치명타 확률 : {effect.effectValue}</b>\n";
                        break;
                    case ArmorEffectType.CritDamageBoost:
                        infoText.text += $"<b>치명타 대미지 : {effect.effectValue}</b>\n";
                        break;
                    case ArmorEffectType.SpeedBoost:
                        infoText.text += $"<b>추가 속도 : {effect.effectValue}</b>\n";
                        break;
                    // 추가적인 효과 
                    default:
                        infoText.text += $"<b>Unknown Effect Type: {effect.effectType}</b>\n";
                        break;
                }
            }

        }
    }

    private void DisplayAccessoryInfo(AcceItem acceInfo, TMP_Text infoText)
    {
        if (acceInfo != null)
        {
            // 방어구 부위 정보 표시
            infoText.text += $"부위: {acceInfo.AcceType}\n";
            foreach (var effect in acceInfo.effectList)
            {
                switch (effect.effectType)
                {
                    case AcceEffectType.AttackBoost:
                        infoText.text += $"<b>추가 공격력: {effect.effectValue}</b>\n";
                        break;
                    case AcceEffectType.MaxHealthBoost:
                        infoText.text += $"<b>추가 체력: {effect.effectValue}</b>\n";
                        break;
                    case AcceEffectType.CritChanceBoost:
                        infoText.text += $"<b>치명타 확률 : {effect.effectValue}</b>\n";
                        break;
                    case AcceEffectType.CritDamageBoost:
                        infoText.text += $"<b>치명타 대미지 : {effect.effectValue}</b>\n";
                        break;
                    case AcceEffectType.SpeedBoost:
                        infoText.text += $"<b>추가 속도 : {effect.effectValue}</b>\n";
                        break;
                    // 추가적인 효과 
                    default:
                        infoText.text += $"<b>Unknown Effect Type: {effect.effectType}</b>\n";
                        break;
                }
            }
        }
    }

    private void DisplayConsumableInfo(ConsumItem consumInfo, TMP_Text infoText)
    {
        if(consumInfo != null)
        {
            if (consumInfo.effectType == EffectType.HealEffect)
            {
                infoText.text += $"<b>회복량 : {consumInfo.EffectPoint}</b>\n";
            }
            else if (consumInfo.effectType == EffectType.AttackBoostEffect)
            {
                infoText.text += $"<b>공격력 증가 : {consumInfo.EffectPoint}</b>\n";
                infoText.text += $"<b>지속 시간 : {consumInfo.EffectDuration}</b>\n";
            }
            else if(consumInfo.effectType == EffectType.SpeedBoostEffect)
            {
                infoText.text += $"<b>속도 증가 : {consumInfo.EffectPoint}</b>\n";
                infoText.text += $"<b>지속 시간 : {consumInfo.EffectDuration}</b>\n";
            }

        }
    }

    private void DisplayMaterialInfo(MaterialItem MaterialInfo, TMP_Text infoText)
    {

    }
    public void OnCloseShop()
    {
        UIManager.Instance.CloseTopUi();
    }
}
