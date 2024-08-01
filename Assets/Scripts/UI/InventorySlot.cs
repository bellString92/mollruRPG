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

public class InventorySlot : MonoBehaviour, IDropHandler, ISetChild , IPointerClickHandler
{
    public GameObject myChild = null;
    public Player user;
    SaveItemInfo itemInfo;
    public SlotType _slotType = SlotType.UseItem;
    public void OnDrop(PointerEventData eventData)
    {
        if (_slotType.Equals(SlotType.Skill)) return;

        var draggedItemInfo = eventData.pointerDrag.GetComponent<SaveItemInfo>();        
        // 드래그 시작 슬롯의 InventorySlot 컴포넌트를 가져옵니다.
        var startSlot = eventData.pointerDrag.GetComponentInParent<InventorySlot>();

        if (myChild != null && draggedItemInfo != null)
        {
            if (draggedItemInfo.item.maxStack > 1)
            {
                var myItemInfo = myChild?.GetComponent<SaveItemInfo>();
                // 드래그한 아이템과 현재 슬롯의 아이템이 같은 종류인지 확인
                if (myItemInfo != null && myItemInfo.item.quantity != myItemInfo.item.maxStack)
                {
                    if (myItemInfo.item.itemID == draggedItemInfo.item.itemID)
                    {
                        int totalQuantity = myItemInfo.item.quantity + draggedItemInfo.item.quantity;

                        if (totalQuantity > myItemInfo.item.maxStack)
                        {
                            int excessQuantity = totalQuantity - myItemInfo.item.maxStack;

                            // 슬롯 아이템 수량을 최대 스택 수량으로 설정
                            myItemInfo.item.quantity = myItemInfo.item.maxStack;

                            // 드래그된 아이템의 수량을 초과한 수량만큼 감소
                            draggedItemInfo.item.quantity = excessQuantity;
                        }
                        else
                        {
                            // 수량을 합친다
                            myItemInfo.item.quantity = totalQuantity;

                            // 드래그 시작 슬롯의 myChild를 null로 설정한다.
                            eventData.pointerDrag.GetComponent<IGetParent>()?.myParent.GetComponent<ISetChild>().SetChild(null);

                            // 드래그된 아이템을 삭제한다
                            Destroy(eventData.pointerDrag);
                        }

                        // 아이템 수량 업데이트
                        TextMeshProUGUI quantityText = myItemInfo.GetComponentInChildren<TextMeshProUGUI>();
                        if (quantityText != null)
                        {
                            quantityText.text = myItemInfo.item.quantity.ToString();
                        }

                        return;// 기존의 코드 실행을 방지하고 여기서 메서드 종료
                    }
                }
            }
        }
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
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.SetDestroySlotItem(myChild != null ? this : null);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {           
            // 우클릭 처리: use 실행
            if (myChild != null)
            {
                itemInfo = myChild?.GetComponent<SaveItemInfo>();
                if (itemInfo.item.itemType == ItemType.weaponItem || itemInfo.item.itemType == ItemType.armorItem || itemInfo.item.itemType == ItemType.acceItem)
                {
                    PlayerStateUiManager stateManager = PlayerStateUiManager.Instance;
                    stateManager.SetSlot(myChild, itemInfo.item.itemType);
                    myChild = null;
                }
                else if (itemInfo?.item.itemType == ItemType.consumItem)
                {
                    itemInfo?.item.Use(user);
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
        IChildObject child = GetComponentInChildren<IChildObject>();
        if (child != null)
        {
            myChild = child.gameObject;
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
