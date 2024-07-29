using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StateUiSlot : MonoBehaviour, IDropHandler, ISetChild, IPointerClickHandler
{
    public GameObject myChild = null;
    private SaveItemInfo ItemInfo; // SaveItemInfo 컴포넌트를 저장할 변수

    [SerializeField] private ItemType allowedItemType; // 인스펙터에서 설정할 수 있는 아이템 타입 변수

    public void SetChild(GameObject newChild)
    {
        myChild = newChild;
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
                if (itemKind.itemType == allowedItemType)
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
            // 우클릭 처리: use 실행
            if (myChild != null)
            {
                Inventory.Instance.AddItem(myChild);
                myChild=null;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        IChildObject child = GetComponentInChildren<IChildObject>();
        if (child != null)
        {
            myChild = child.gameObject;
            ItemInfo = myChild.GetComponent<SaveItemInfo>();
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
