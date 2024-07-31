using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StateUiSlot : MonoBehaviour, IDropHandler, ISetChild, IPointerClickHandler
{
    public GameObject myChild = null;
    public GameObject curChild = null;
    private SaveItemInfo ItemInfo; // SaveItemInfo 컴포넌트를 저장할 변수
    private Player user; 

    [SerializeField] private ItemType allowedItemType; // 인스펙터에서 설정할 수 있는 아이템 타입 변수
    [SerializeField] private ArmorType allowedArmorType; // 인스펙터에서 설정할 수 있는 방어구 타입 변수
    [SerializeField] private AcceType allowedAcceType; // 인스펙터에서 설정할 수 있는 방어구 타입 변수


    public void SetChild(GameObject newChild)
    {
        myChild = newChild;
    }

    void Start()
    {
        IChildObject child = GetComponentInChildren<IChildObject>();
        if (child != null)
        {
            myChild = child.gameObject;
            curChild = myChild;
            ItemInfo = myChild.GetComponent<SaveItemInfo>();
        }
        if (user == null)
        {
            user = PlayerStateUiManager.Instance.user;
        }
        Debug.Log($"allowedArmorType in Start: {allowedArmorType}");
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
                bool isAllowed = false;

                // 아이템 타입이 슬롯의 허용된 타입과 같은지 확인합니다.
                if (itemKind.itemType == allowedItemType)
                {
                    Debug.Log($"Allowed Item Type: {allowedItemType}, Dragged Item Type: {itemKind.itemType}");

                    if (itemKind is ArmorItem armorItem)
                    {
                        isAllowed = armorItem.armorType == allowedArmorType;
                        Debug.Log($"Allowed Armor Type: {allowedArmorType}, Dragged Armor Type: {armorItem.armorType}");
                    }
                    else if (itemKind is AcceItem acceItem)
                    {
                        isAllowed = acceItem.AcceType == allowedAcceType;
                        Debug.Log($"Allowed Acce Type: {allowedAcceType}, Dragged Acce Type: {acceItem.AcceType}");
                    }
                    else
                    {
                        isAllowed = true; // 기본적으로 허용
                    }
                }


                if (isAllowed)
                {
                    // 기존 아이템 처리
                    if (myChild != null)
                    {
                        myChild.GetComponent<ISwapParent>()?.SwapParent(draggedItem.GetComponent<IGetParent>().myParent);

                        ItemInfo?.itemKind.TakeOff(user);// 기존 아이템 능력치 해제
                    }
                    else
                    {
                        draggedItem.GetComponent<IGetParent>()?.myParent.GetComponent<ISetChild>().SetChild(null);
                    }

                    // 새 아이템 설정
                    draggedItem.GetComponent<IChangeParent>()?.ChangeParent(transform);
                    myChild = draggedItem;
                    myChild.GetComponent<IChangeParent>()?.ChangeParent(transform);
                    ItemInfo = myChild.GetComponent<SaveItemInfo>();

                    ItemInfo?.itemKind.Use(user); // 새 아이템 능력치 적용
                    curChild = myChild;
                    //  PlayerStateUiManager.Instance.UpdatePlayerStats();// 능력치 업데이트
                }
                else
                {
                    Debug.LogWarning("Invalid item type.");
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
            ItemInfo = myChild.GetComponent<SaveItemInfo>();
            // 우클릭 처리
            if (myChild != null)
            {
                ItemInfo?.itemKind.TakeOff(user);
                Inventory.Instance.AddItem(myChild);
                myChild=null;
                curChild = myChild;
                // 능력치 업데이트
              //  PlayerStateUiManager.Instance.UpdatePlayerStats();
            }
        }
    }

    public void CheckAndTakeOffIfNeeded(GameObject draggedItem)
    {     
        if (myChild == null)
        {
            ItemInfo = draggedItem.GetComponent<SaveItemInfo>();
            ItemInfo?.itemKind.TakeOff(user);
           // PlayerStateUiManager.Instance.UpdatePlayerStats();
        }
        else if(curChild != myChild)
        {
            myChild.GetComponent<SaveItemInfo>().itemKind.Use(user);
            curChild.GetComponent<SaveItemInfo>().itemKind.TakeOff(user);
           // PlayerStateUiManager.Instance.UpdatePlayerStats();
        }
        curChild = myChild;
    }
}
