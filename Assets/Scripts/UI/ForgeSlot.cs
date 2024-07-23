using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ForgeSlot : MonoBehaviour, IDropHandler
{
    public GameObject myChild = null;
    private SaveItemInfo ItemInfo; // SaveItemInfo 컴포넌트를 저장할 변수
    // Start is called before the first frame update
    void Start()
    {
        IChildObject child = GetComponentInChildren<IChildObject>();
        if (child != null)
        {
            myChild = child.gameObject;
            ItemInfo = myChild.GetComponent<SaveItemInfo>();
            ForgeUI.Instance.DisplayItemInfo(ItemInfo);
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
                var itemKind = draggedItemInfo.itemKind;

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
                    ForgeUI.Instance.DisplayItemInfo(ItemInfo);
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

    public void OnIncreaseKaiLevelButtonClick()
    {
        if (ItemInfo != null)
        {
            ForgeUI.Instance.IncreaseKaiLevel(ItemInfo);
        }
    }
}
