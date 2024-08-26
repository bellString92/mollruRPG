using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

interface IChildObject
{
    GameObject gameObject
    {
        get;
    }
}

interface ISetChild
{
    void SetChild(GameObject newChild);
}

public enum SlotType
{
    UseItem, Skill, SlotItem, SlotSkill, Item
}

public class InventorySlot : MonoBehaviour, IDropHandler, ISetChild, IPointerClickHandler
{
    public GameObject myChild = null;
    public Player user;
    SaveItemInfo itemInfo;
    public SlotType _slotType = SlotType.UseItem;

    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 1.0f; // 더블 클릭 감지 시간(초)

    public void OnDrop(PointerEventData eventData)
    {
        // 스킬 슬롯은 무시
        if (_slotType.Equals(SlotType.Skill)) return;

        // 드래그된 아이템 정보 가져오기
        var draggedItemInfo = eventData.pointerDrag?.GetComponent<SaveItemInfo>();
        if (draggedItemInfo == null) return;

        // 드래그된 아이템의 슬롯 타입 가져오기
        var draggedSlotType = eventData.pointerDrag.GetComponent<Drag>().slotType;

        // 현재 슬롯의 아이템 정보 가져오기
        var myItemInfo = myChild?.GetComponent<SaveItemInfo>();

        // 슬롯 타입이 다르면 처리하지 않음 - 퀵슬롯에서 드래그한 아이템이 인벤슬롯 아이템위에 드랍할때 상호작용 방지
        if (_slotType != draggedSlotType)
        {
            return;
        }

        // 플레이어 상태 UI가 활성화된 경우에만 추가 검사
        if (PlayerStateUiManager.Instance.gameObject.activeSelf)
        {
            if (!IsValidItemType(draggedItemInfo, myItemInfo))
            {
                return;
            }
        }

        // 스택 가능한 아이템 처리
        if (draggedItemInfo.item.maxStack > 1 && TryStackItems(draggedItemInfo, myItemInfo, eventData))
        {
            return; // 스택 처리 완료 후 종료
        }

        // 부모 교체 및 드래그된 아이템 처리
        HandleItemSwap(eventData, draggedItemInfo);
    }

    // 아이템 타입과 관련된 검증 로직 분리
    private bool IsValidItemType(SaveItemInfo draggedItemInfo, SaveItemInfo myItemInfo)
    {
        var dragItemType = draggedItemInfo.item.itemType;

        // 드래그된 아이템이 무기, 방어구, 악세사리인 경우에만 검사
        if (dragItemType != ItemType.weaponItem && dragItemType != ItemType.armorItem && dragItemType != ItemType.acceItem)
        {
            Debug.LogWarning("유효하지 않은 아이템 타입입니다.");
            return false;
        }

        // 슬롯에 아이템이 없을 경우 유효
        if (myItemInfo == null)
        {
            return true;
        }

        // 아이템 타입이 일치하지 않는 경우 처리
        if (dragItemType != myItemInfo.item.itemType)
        {
            Debug.LogWarning("아이템 타입이 일치하지 않습니다.");
            return false;
        }

        // 방어구 타입 검사
        if (dragItemType == ItemType.armorItem)
        {
            var draggedArmor = draggedItemInfo.item as ArmorItem;
            var myArmor = myItemInfo.item as ArmorItem;
            if (draggedArmor?.armorType != myArmor?.armorType)
            {
                Debug.LogWarning("아머 타입이 일치하지 않습니다.");
                return false;
            }
        }
        // 악세사리 타입 검사
        else if (dragItemType == ItemType.acceItem)
        {
            var draggedAcce = draggedItemInfo.item as AcceItem;
            var myAcce = myItemInfo.item as AcceItem;
            if (draggedAcce?.AcceType != myAcce?.AcceType)
            {
                Debug.LogWarning("악세사리 타입이 일치하지 않습니다.");
                return false;
            }
        }

        return true; // 모든 검사 통과
    }

    // 스택 가능한 아이템 처리 로직 분리
    private bool TryStackItems(SaveItemInfo draggedItemInfo, SaveItemInfo myItemInfo, PointerEventData eventData)
    {
        if (myItemInfo != null && myItemInfo.item.itemID == draggedItemInfo.item.itemID && myItemInfo.item.quantity != myItemInfo.item.maxStack)
        {
            int totalQuantity = myItemInfo.item.quantity + draggedItemInfo.item.quantity;

            if (totalQuantity > myItemInfo.item.maxStack)
            {
                int excessQuantity = totalQuantity - myItemInfo.item.maxStack;
                myItemInfo.item.quantity = myItemInfo.item.maxStack;
                draggedItemInfo.item.quantity = excessQuantity;
            }
            else
            {
                myItemInfo.item.quantity = totalQuantity;
                eventData.pointerDrag.GetComponent<IGetParent>()?.myParent.GetComponent<ISetChild>().SetChild(null);
                Destroy(eventData.pointerDrag);
            }

            // 수량 업데이트
            UpdateItemQuantity(myItemInfo);
            return true; // 스택 처리 성공
        }

        return false; // 스택 처리 실패
    }

    // 아이템 수량 업데이트 로직
    private void UpdateItemQuantity(SaveItemInfo itemInfo)
    {
        var quantityText = itemInfo.GetComponentInChildren<TextMeshProUGUI>();
        if (quantityText != null)
        {
            quantityText.text = itemInfo.item.quantity.ToString();
        }
    }

    // 부모 교체 및 아이템 드랍 처리
    private void HandleItemSwap(PointerEventData eventData, SaveItemInfo draggedItemInfo)
    {
        if (myChild != null)
        {
            myChild.GetComponent<ISwapParent>()?.SwapParent(eventData.pointerDrag.GetComponent<IGetParent>().myParent);
        }
        else
        {
            eventData.pointerDrag.GetComponent<IGetParent>()?.myParent.GetComponent<ISetChild>().SetChild(null);
        }

        myChild = eventData.pointerDrag;
        myChild.GetComponent<IChangeParent>()?.ChangeParent(transform);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ShopManager shopManager = ShopManager.Instance;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (shopManager.gameObject.activeSelf)
            {
                shopManager.SetDestroySlotItem(myChild != null ? this : null);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 우클릭 처리: use 실행
            if (myChild != null)
            {
                PlayerStateUiManager stateManager = PlayerStateUiManager.Instance;
                ForgeManager forgeManager = ForgeManager.Instance;
                Inventory invenManager = Inventory.Instance;
                itemInfo = myChild?.GetComponent<SaveItemInfo>();

                float timeSinceLastClick = Time.time - lastClickTime;
                lastClickTime = Time.time;

                if (timeSinceLastClick <= doubleClickThreshold)
                {
                    if (shopManager != null)
                    {
                        if (shopManager.gameObject.activeSelf)
                        {
                            shopManager.SetDestroySlotItem(myChild != null ? this : null);
                            shopManager.OnDestroyInventorySlotItem();
                            return; // 더블 클릭 시에는 이후 로직을 수행하지 않음
                        }
                    }
                }
                if (forgeManager != null)
                {
                    if (forgeManager.gameObject.activeSelf)
                    {
                        if (itemInfo.item.itemType == ItemType.weaponItem || itemInfo.item.itemType == ItemType.armorItem)
                        {
                            forgeManager.SetSlot(myChild);
                            myChild = null;
                            return;
                        }
                    }
                }
                if (!shopManager.gameObject.activeSelf && !forgeManager.gameObject.activeSelf)
                {
                    if (itemInfo.item.itemType == ItemType.weaponItem || itemInfo.item.itemType == ItemType.armorItem || itemInfo.item.itemType == ItemType.acceItem)
                    {
                        if (stateManager.CheckSlotEmpty(myChild, itemInfo.item.itemType))
                        {
                            stateManager.SetSlot(myChild, itemInfo.item.itemType);
                            myChild = null;
                        }
                        else if (invenManager.HasEmptySlot())
                        {
                            stateManager.SetSlot(myChild, itemInfo.item.itemType);
                            myChild = null;
                        }
                        else
                        {
                            invenManager.NoEmptySlot();
                            return;
                        }
                    }
                    else if (itemInfo?.item.itemType == ItemType.consumItem)
                    {
                        itemInfo?.UseItem(user);
                        if (itemInfo?.item.quantity == 0)
                        {
                            Destroy(myChild);
                        }
                    }
                    else
                    {
                        Debug.Log("올바르지 않은 아이템 사용");
                    }
                }
            }
        }
    }

    public void SetChild(GameObject newChild)
    {
        myChild = newChild;
    }
    public void DestroyChild() // 마이차일드 비우기
    {
        if (myChild != null)
        {
            Destroy(myChild);
            myChild = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SaveItemInfo child = GetComponentInChildren<SaveItemInfo>();
        if (child != null)
        {
            myChild = child.gameObject;
        }
        else
        {
            myChild = null;
        }
        if (user == null && Inventory.Instance != null)
        {
            user = Inventory.Instance.user;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
