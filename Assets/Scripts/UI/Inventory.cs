using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public GameObject NoEmptySlotPopup;
    public Player user;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public Transform content; // 슬롯들을 자식으로 가지고 있는 부모 Transform


    // 아이템을 생성하는 메서드
    public void CreateItem(ItemKind itemKind, GameObject itemPrefab)
    {
        if (itemKind != null && itemPrefab != null)
        {
            // content의 자식 슬롯들을 순회
            foreach (Transform slot in content)
            {
                InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
                if (inventorySlot != null && inventorySlot.myChild == null)
                {
                    // ItemKind를 복제하여 사용
                    ItemKind newItemKind = Instantiate(itemKind);

                    // 새로운 게임 오브젝트 생성 및 설정
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
                    saveItemInfo.itemKind = newItemKind;

                    // 태그 설정
                    newItem.tag = newItemKind.itemTag;

                    // 아이콘 설정
                    Image iconImage = newItem.GetComponent<Image>();
                    if (iconImage != null && newItemKind.itemIcon != null)
                    {
                        iconImage.sprite = newItemKind.itemIcon;
                    }

                    // 슬롯에 아이템 배치
                    inventorySlot.SetChild(newItem);
                    newItem.transform.SetParent(slot);

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
            }
            Debug.LogWarning("No empty slots available.");
            UIManager.Instance.ShowUI(NoEmptySlotPopup);

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
            // content의 자식 슬롯들을 순회
            foreach (Transform slot in content)
            {
                InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
                if (inventorySlot != null && inventorySlot.myChild == null)
                {
                    item.transform.SetParent(slot);
                    item.transform.localPosition = Vector2.zero;
                    inventorySlot.SetChild(item);
                    break;
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
                if (saveItemInfo != null && saveItemInfo.itemKind.itemID == itemKind.itemID)
                {
                    totalQuantity += saveItemInfo.itemKind.quantity;

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
                if (saveItemInfo != null && saveItemInfo.itemKind.itemID == itemKind.itemID)
                {
                    if (saveItemInfo.itemKind.quantity >= remainingQuantity)
                    {
                        // 현재 슬롯의 아이템 수량이 요구 수량보다 크거나 같은 경우
                        saveItemInfo.itemKind.quantity -= remainingQuantity;
                        remainingQuantity = 0;
                    }
                    else
                    {
                        // 현재 슬롯의 아이템 수량이 요구 수량보다 작은 경우
                        remainingQuantity -= saveItemInfo.itemKind.quantity;
                        saveItemInfo.itemKind.quantity = 0;
                    }

                    // 아이템 수량 업데이트
                    TextMeshProUGUI quantityText = saveItemInfo.GetComponentInChildren<TextMeshProUGUI>();
                    if (quantityText != null)
                    {
                        quantityText.text = saveItemInfo.itemKind.quantity.ToString();
                    }

                    // 수량이 0이면 아이템 제거
                    if (saveItemInfo.itemKind.quantity <= 0)
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
}

