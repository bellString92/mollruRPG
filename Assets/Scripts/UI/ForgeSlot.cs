using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ForgeSlot : MonoBehaviour, IDropHandler, ISetChild, IPointerClickHandler
{
    public GameObject myChild = null;
    public SlotType slotType;
    private SaveItemInfo ItemInfo; // SaveItemInfo 컴포넌트를 저장할 변수

    private bool isEmpty = true;

    // Start is called before the first frame update
    void Start()
    {
        IChildObject child = GetComponentInChildren<IChildObject>();
        if (child != null)
        {
            myChild = child.gameObject;
            ItemInfo = myChild.GetComponent<SaveItemInfo>();
            ForgeManager.Instance.DisplayItemInfo(ItemInfo);
        }
        
    }
    private void Update()
    {
        if (myChild != null)
        {
            isEmpty = false;
        }

        if (myChild == null && isEmpty == false)
        {
            isEmpty = true;
            ClearSlot();
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag;
        if (draggedItem != null)
        {
            var draggedItemInfo = draggedItem.GetComponent<SaveItemInfo>();

            // 드래그된 아이템의 ItemInfo가 null이 아닌지 확인합니다.
            if (draggedItemInfo != null)
            {
                var itemKind = draggedItemInfo.item;

                // 아이템 타입이 WeaponItem 또는 ArmorItem인지 확인합니다.
                if (itemKind.itemType == ItemType.weaponItem || itemKind.itemType == ItemType.armorItem)
                {
                    // 기존 아이템 처리
                    if (myChild != null)
                    {
                        myChild.GetComponent<ISwapParent>()?.SwapParent(draggedItem.GetComponent<IGetParent>().myParent);
                    }
                    else
                    {
                        draggedItem.GetComponent<IGetParent>()?.myParent.GetComponent<ISetChild>().SetChild(null);
                    }

                    // 새 아이템 설정
                    myChild = draggedItem;
                    myChild.GetComponent<IChangeParent>()?.ChangeParent(transform);
                    ItemInfo = myChild.GetComponent<SaveItemInfo>();

                    // 아이템 정보를 표시합니다.
                    ForgeManager.Instance.DisplayItemInfo(ItemInfo);
                }
                else
                {
                    Debug.LogWarning("Invalid item type. Only WeaponItem and ArmorItem are allowed.");
                }
            }
            else
            {
                Debug.LogWarning("Dragged item does not have SaveItemInfo component.");
            }
        }
        else
        {
            Debug.LogWarning("No item dragged.");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (myChild != null)
            {
                ClearSlot();
            }
        }
    }

    public void SetChild(GameObject newChild)
    {
        myChild = newChild;
    }
    public void UpdateItemSlotInfo()
    {
        ItemInfo = myChild.GetComponent<SaveItemInfo>();
    }

    public void OnIncreaseKaiLevelButtonClick()
    {
        if (ItemInfo != null)
        {
            ForgeManager.Instance.IncreaseKaiLevel(ItemInfo);
        }
    }

    public void ClearSlot()
    {
        ForgeManager forgeManager = ForgeManager.Instance;

        Inventory.Instance.AddItem(myChild);
        myChild = null;
        foreach (Transform child in forgeManager.scrollViewContent)
        {
            Destroy(child.gameObject);
        }
        forgeManager.UpdateLuckPercent(null);
        forgeManager.materialRequirementDisplay.DisplayMaterialRequirements(null);
    }
}
