using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public GameObject NoEmptySlotPopup;
    public Player user;
    public Transform dragItem;
    public TextMeshProUGUI haveGold;
    private int curGold;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public Transform content; // 슬롯들을 자식으로 가지고 있는 부모 Transform

    private void Start()
    {
        UpdateGold();
    }
    private void Update()
    {
        if (curGold != user.myStat.myGold)
        {
            UpdateGold();
        }
    }
    private void UpdateGold()
    {
        haveGold.text = user.myStat.myGold.ToString();
        curGold = user.myStat.myGold;
    }

    // 아이템을 생성하는 메서드
    public void CreateItem(ItemKind itemKind, GameObject itemPrefab)
    {
        if (itemKind != null && itemPrefab != null)
        {
            if (TryAddItemToExistingStack(itemKind)) // 같은 아이템이 있는지 확인
            {
                return; // 만약 새로 생성할 아이템의 갯수 기존에 있던 아이템의 갯수를 더한 값이 아이템의 최대 스텍을 넘지못하면 생성하지 않고 종료  
            }

            if (HasEmptySlot())// 빈슬롯이 있는지 검사
            {
                ItemKind newItemKind = Instantiate(itemKind);
                GameObject newItem = Instantiate(itemPrefab);
                newItem.name = newItemKind.itemName;


                // 새로운 게임 오브젝트에 Drag 컴포넌트 추가 (이미 존재하지 않을 때만)
                if (newItem.GetComponent<DragItem>() == null)
                {
                    newItem.AddComponent<DragItem>();
                }

                // 아이템의 ItemKind 정보를 불러오기를 쉽게하기 위해 정보를 저장 (이미 존재하지 않을 때만)
                SaveItemInfo saveItemInfo = newItem.GetComponent<SaveItemInfo>();
                if (saveItemInfo == null)
                {
                    saveItemInfo = newItem.AddComponent<SaveItemInfo>();
                }
                saveItemInfo.item = newItemKind;

                // 태그 설정
                newItem.tag = newItemKind.itemTag;

                // 아이콘 설정
                Image iconImage = newItem.GetComponentInChildren<Image>();
                if (iconImage != null && newItemKind.itemIcon != null)
                {
                    iconImage.sprite = newItemKind.itemIcon;
                }

                //레어도에 따라 차이점
                Image rarityBG = newItem.AddComponent<Image>();
                switch (saveItemInfo.item.rarity)
                {
                    case Rarity.Common:
                        rarityBG.color = Color.clear;
                        break;
                    case Rarity.Uncommon:
                        rarityBG.color = new Color(0, 1, 0, 10f / 255f);
                        break;
                    case Rarity.Rare:
                        rarityBG.color = new Color(0f, 0f, 1f, 10f / 255f);
                        break;
                    case Rarity.Epic:
                        rarityBG.color = new Color(1, 0, 1, 10f / 255f);
                        break;
                    case Rarity.Legendary:
                        rarityBG.color = new Color(1, 1, 0, 10f / 255f);
                        break;
                    default:
                        break;
                }

                // 슬롯에 아이템 배치
                AddItemToEmptySlot(newItem);

                //아이템 갯수 표시
                TextMeshProUGUI quantityText = newItem.GetComponentInChildren<TextMeshProUGUI>();
                if (quantityText != null)
                {
                    quantityText.text = newItemKind.quantity.ToString();
                }

                // 위치와 크기 설정
                // RectTransform 초기화
                RectTransform rectTransform = newItem.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.sizeDelta = Vector2.zero;
                    rectTransform.localPosition = Vector3.zero;
                    rectTransform.localRotation = Quaternion.identity;
                    rectTransform.localScale = Vector3.one;
                }

                return;


            }
            Debug.LogWarning("No empty slots available.");
            NoEmptySlot();

        }
        else
        {
            Debug.LogWarning("Invalid item kind or item prefab.");
        }
    }
    public void AddItem(GameObject item)
    {
        if (item != null)
        {
            if (item != null)
            {
                SaveItemInfo saveItemInfo = item.GetComponent<SaveItemInfo>();
                if (saveItemInfo != null && saveItemInfo.item != null)
                {
                    ItemKind itemKind = saveItemInfo.item;
                    if (TryAddItemToExistingStack(itemKind))
                    {
                        Destroy(item); // 아이템이 성공적으로 추가된 경우 파괴
                        return;
                    }
                    if (HasEmptySlot())
                    {
                        AddItemToEmptySlot(item);
                    }
                    else
                    {
                        NoEmptySlot();
                    }
                }
                else
                {
                    Debug.LogWarning("Invalid item information.");
                }
            }
        }
    }
    // 인벤토리에 특정 아이템과 수량이 있는지 확인하는 메서드
    public bool HasItem(ItemKind itemKind, int quantity)
    {
        int totalQuantity = 0;

        foreach (Transform slot in content)
        {
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
            if (inventorySlot != null && inventorySlot.myChild != null)
            {
                SaveItemInfo saveItemInfo = inventorySlot.myChild.GetComponent<SaveItemInfo>();
                if (saveItemInfo != null && saveItemInfo.item.itemID == itemKind.itemID)
                {
                    totalQuantity += saveItemInfo.item.quantity;

                    // 만약 합산된 수량이 요구 수량 이상이면 true 반환
                    if (totalQuantity >= quantity)
                    {
                        return true;
                    }
                }
            }
        }

        // 합산된 수량이 요구 수량에 못 미치면 false 반환
        return false;
    }
    public int GetItemQuantity(ItemKind itemKind) // 아이템의 총 갯수를 반환
    {
        int totalQuantity = 0;

        foreach (Transform slot in content)
        {
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
            if (inventorySlot != null && inventorySlot.myChild != null)
            {
                SaveItemInfo saveItemInfo = inventorySlot.myChild.GetComponent<SaveItemInfo>();
                if (saveItemInfo != null && saveItemInfo.item.itemID == itemKind.itemID)
                {
                    totalQuantity += saveItemInfo.item.quantity;
                }
            }
        }

        return totalQuantity;
    }

    // 인벤토리에서 특정 아이템과 수량을 제거하는 메서드
    public void RemoveItem(ItemKind itemKind, int quantity)
    {
        int remainingQuantity = quantity;

        foreach (Transform slot in content)
        {
            if (remainingQuantity <= 0) break;

            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
            if (inventorySlot != null && inventorySlot.myChild != null)
            {
                SaveItemInfo saveItemInfo = inventorySlot.myChild.GetComponent<SaveItemInfo>();
                if (saveItemInfo != null && saveItemInfo.item.itemID == itemKind.itemID)
                {
                    if (saveItemInfo.item.quantity >= remainingQuantity)
                    {
                        // 현재 슬롯의 아이템 수량이 요구 수량보다 크거나 같은 경우
                        saveItemInfo.item.quantity -= remainingQuantity;
                        remainingQuantity = 0;
                    }
                    else
                    {
                        // 현재 슬롯의 아이템 수량이 요구 수량보다 작은 경우
                        remainingQuantity -= saveItemInfo.item.quantity;
                        saveItemInfo.item.quantity = 0;
                    }

                    // 아이템 수량 업데이트
                    TextMeshProUGUI quantityText = saveItemInfo.GetComponentInChildren<TextMeshProUGUI>();
                    if (quantityText != null)
                    {
                        quantityText.text = saveItemInfo.item.quantity.ToString();
                    }

                    // 수량이 0이면 아이템 제거
                    if (saveItemInfo.item.quantity <= 0)
                    {
                        Destroy(inventorySlot.myChild);
                        inventorySlot.SetChild(null);
                    }
                }
            }
        }

        if (remainingQuantity > 0)
        {
            Debug.LogWarning("Item not found or insufficient quantity.");
        }
    }
    public bool HasEmptySlot()
    {
        foreach (Transform slot in content)
        {
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
            if (inventorySlot != null && inventorySlot.myChild == null)
            {
                return true; // 빈 슬롯을 찾음
            }
        }
        return false; // 빈 슬롯이 없음
    }

    public bool TryAddItemToExistingStack(ItemKind itemKind)
    {
        foreach (Transform slot in content)
        {
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
            if (inventorySlot != null && inventorySlot.myChild != null)
            {
                SaveItemInfo existingItemInfo = inventorySlot.myChild.GetComponent<SaveItemInfo>();
                if (existingItemInfo != null && existingItemInfo.item != null)
                {
                    if ((itemKind.itemType == ItemType.consumItem || itemKind.itemType == ItemType.materialItem) && existingItemInfo.item.itemID == itemKind.itemID)
                    {
                        int combinedQuantity = existingItemInfo.item.quantity + itemKind.quantity;
                        if (combinedQuantity <= itemKind.maxStack)
                        {
                            existingItemInfo.item.quantity = combinedQuantity;
                            TextMeshProUGUI quantityText = inventorySlot.myChild.GetComponentInChildren<TextMeshProUGUI>();
                            if (quantityText != null)
                            {
                                quantityText.text = existingItemInfo.item.quantity.ToString();
                            }
                            return true;
                        }
                        else
                        {
                            int remainingQuantity = combinedQuantity - itemKind.maxStack;
                            existingItemInfo.item.quantity = itemKind.maxStack;
                            TextMeshProUGUI quantityText = inventorySlot.myChild.GetComponentInChildren<TextMeshProUGUI>();
                            if (quantityText != null)
                            {
                                quantityText.text = existingItemInfo.item.quantity.ToString();
                            }
                            itemKind.quantity = remainingQuantity;
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool AddItemToEmptySlot(GameObject item)
    {
        foreach (Transform slot in content)
        {
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
            if (inventorySlot != null && inventorySlot.myChild == null)
            {
                item.transform.SetParent(slot);
                item.transform.localPosition = Vector2.zero;
                inventorySlot.SetChild(item);
                return true;
            }
        }
        return false;
    }

    public void NoEmptySlot()
    {
        UIManager.Instance.ShowOkbuttonUI(NoEmptySlotPopup, OkBoxType.NoEmptySlot);
    }
}

