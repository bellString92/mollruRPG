using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSkillSlot : MonoBehaviour, IDropHandler, ISetChild, IPointerClickHandler
{
    public GameObject myChild = null;
    public SlotType slotType = SlotType.UseItem;
    public Transform dragItemSkill;
    public SaveItemInfo originalItemInfo = null;  // 원본 아이템 정보 저장
    private SaveItemInfo slotItemInfo;
    private ItemKind itemInfo = null;

    private void Update()
    {
        // 원본 아이템이 없거나 수량이 0 이하일 경우 인벤토리에서 동일한 아이템을 찾아 재등록
        if (originalItemInfo == null && slotItemInfo != null)
        {
            SaveItemInfo newItemInfo = FindNextItemWithSameID(slotItemInfo.item.itemID.ToString());
            if (newItemInfo != null)
            {
                originalItemInfo = newItemInfo; // 새 아이템으로 등록
                slotItemInfo.item.quantity = originalItemInfo.item.quantity;
            }
            else
            {
                // 동일한 아이템을 찾지 못한 경우, 슬롯에서 아이템 삭제
                if (myChild != null)
                {
                    Destroy(myChild);
                    myChild = null;
                }
            }
        }
        else
        {
            // 슬롯 아이템의 수량이 변경되면 업데이트
            if (slotItemInfo != null && slotItemInfo.item.quantity != Inventory.Instance.GetItemQuantity(slotItemInfo.item))
            {
                slotItemInfo.item.quantity = Inventory.Instance.GetItemQuantity(slotItemInfo.item);
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Drag dragComponent = eventData.pointerDrag.GetComponent<Drag>();
        SlotType draggedSlotType = dragComponent.slotType;
        SaveItemInfo draggedItemInfo = eventData.pointerDrag.GetComponent<SaveItemInfo>();

        // QuickSlotManager에서 동일한 아이템이 이미 등록된 슬롯을 찾음
        ItemSkillSlot existingSlot = QuickSlotManager.Instance.FindSlotByItemID(draggedItemInfo.item.itemID);
        if (existingSlot != null && existingSlot != this)
        {
            // 동일한 아이템이 이미 등록된 경우 기존 슬롯의 아이템을 드랍된 슬롯으로 이동
            existingSlot.myChild.transform.SetParent(this.transform);
            this.myChild = existingSlot.myChild;
            existingSlot.myChild = null;
            SetRectTransform(this.myChild.GetComponent<RectTransform>());
            this.myChild.GetComponent<Drag>().destroyChk = false;

            return; // 이미 등록된 경우, 새로운 아이템을 추가하지 않음
        }

        if (slotType.Equals(draggedSlotType) ||
            (slotType.Equals(SlotType.SlotItem) && draggedSlotType.Equals(SlotType.UseItem)) ||
            (slotType.Equals(SlotType.SlotSkill) && draggedSlotType.Equals(SlotType.Skill)))
        {
            if (myChild != null)
            {
                SaveItemInfo currentSaveItemInfo = myChild.GetComponent<SaveItemInfo>();
                if (currentSaveItemInfo != null && draggedItemInfo != null &&
                    currentSaveItemInfo.item.itemID == draggedItemInfo.item.itemID)
                {
                    // 기존에 등록된 동일 아이템이 있을 경우 새 아이템으로 교체하지 않고 무시
                    return;
                }
            }

            // 새로운 아이템을 퀵슬롯에 등록
            Transform slotItem = GameObject.Instantiate(eventData.pointerDrag.transform);
            slotItem.name = eventData.pointerDrag.transform.name;
            slotItem.GetComponent<Drag>().slotType = slotType.Equals(SlotType.UseItem) || slotType.Equals(SlotType.SlotItem) ? SlotType.SlotItem : SlotType.SlotSkill;
            slotItemInfo = slotItem.GetComponent<SaveItemInfo>();

            // 원본 아이템 정보 저장
            originalItemInfo = draggedItemInfo;

            if (originalItemInfo != null)
            {
                if (originalItemInfo.item is ConsumItem)
                {
                    itemInfo = new ConsumItem((ConsumItem)originalItemInfo.item);
                }
                else
                {
                    Debug.Log("잘못된 아이템 형식");
                    return;
                }
                slotItemInfo.item = itemInfo;
                slotItemInfo.item.quantity = Inventory.Instance.GetItemQuantity(slotItemInfo.item); // 인벤토리에서 동일한 아이템 수량 합산
            }

            if (draggedItemInfo != null)
            {
                if (draggedItemInfo.item.itemType == ItemType.consumItem)
                {
                    PlayerSkill skillComponent = slotItem.GetComponent<PlayerSkill>();
                    if (skillComponent != null && skillComponent.comboSkill != null)
                    {
                        Transform comboSkill = GameObject.Instantiate(skillComponent.comboSkill);
                        comboSkill.name = skillComponent.comboSkill.name;
                        skillComponent.comboSkill = comboSkill;
                        comboSkill.gameObject.SetActive(false);
                        comboSkill.SetParent(slotItem);
                        SetRectTransform(comboSkill as RectTransform);
                    }
                }
                else
                {
                    Debug.LogWarning("올바르지 않은 아이템 타입입니다.");
                    Destroy(slotItem.gameObject);
                    return;
                }
            }

            // 설정된 슬롯에 아이템 배치
            slotItem.GetComponent<Image>().raycastTarget = true;
            slotItem.SetParent(transform);
            SetRectTransform(slotItem as RectTransform);

            if (myChild != null)
            {
                Destroy(myChild);
            }

            myChild = slotItem.gameObject;
            myChild.GetComponent<Drag>().destroyChk = false;

            // QuickSlot 등록 시 추가 업데이트 로직 (스킬 컨트롤러에 대한 설정)
            if (slotType.Equals(SlotType.Skill) || slotType.Equals(SlotType.SlotSkill))
            {
                Transform parentTransform = transform.parent;
                int siblingIndex = transform.GetSiblingIndex();
                parentTransform.GetComponent<SkillController>().myImg[siblingIndex] = myChild.GetComponentsInChildren<Image>()[1];
                parentTransform.GetComponent<SkillController>().myLabel[siblingIndex] = myChild.GetComponentsInChildren<TMPro.TMP_Text>()[0];
            }
        }
        else if ((slotType.Equals(SlotType.SlotItem) && draggedSlotType.Equals(SlotType.SlotSkill)) ||
                 (slotType.Equals(SlotType.SlotSkill) && draggedSlotType.Equals(SlotType.SlotItem)))
        {
            dragComponent.destroyChk = false;
        }
    }

    private void SetRectTransform(RectTransform rt)
    {
        Vector2 anchor = new Vector2(0, 0);
        rt.anchorMin = anchor;
        anchor.x = 1; anchor.y = 1;
        rt.anchorMax = anchor;
        anchor.x = 0.5f; anchor.y = 0.5f;
        rt.pivot = anchor;
        anchor.x = 0; anchor.y = 0;
        rt.offsetMin = anchor;
        rt.offsetMax = anchor;
    }

    public void SetChild(GameObject newChild)
    {
        myChild = newChild;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            UseItem();
        }
    }

    public void UseItem()
    {
        if (myChild == null) return;

        SaveItemInfo saveItemInfo = myChild.GetComponent<SaveItemInfo>();
        if (saveItemInfo != null)
        {
            if (saveItemInfo.item is ConsumItem)
            {
                // 아이템 사용
                originalItemInfo.UseItem(UIManager.Instance.player.GetComponent<Player>());
            }
        }
    }

    private SaveItemInfo FindNextItemWithSameID(string itemID)
    {
        foreach (Transform slot in Inventory.Instance.content)
        {
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
            if (inventorySlot != null && inventorySlot.myChild != null)
            {
                SaveItemInfo nextItemInfo = inventorySlot.myChild.GetComponent<SaveItemInfo>();
                if (nextItemInfo != null && nextItemInfo.item.itemID.ToString() == itemID)
                {
                    return nextItemInfo; // 동일한 ID의 아이템 반환
                }
            }
        }
        return null;
    }
}