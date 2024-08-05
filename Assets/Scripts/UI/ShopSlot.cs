using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon; // 아이템 아이콘을 표시할 Image
    public ItemKind item; // 해당 슬롯에 들어있는 아이템 정보

    public Image myFrame; // 선택시 강조될 곳
    public Material highlight;// 선택 강조해줄 발광 머터리얼

    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 1.0f; // 더블 클릭 감지 시간(초)

    // 슬롯 초기화 함수
    public void InitializeSlot(ItemKind newItem)
    {
        item = newItem;
        if (item != null) 
        {
            icon.sprite = item.itemIcon;
            icon.enabled = true;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ShopManager.Instance.ShowInfo(item,this);
            SelectSlotCheck();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            lastClickTime = Time.time;
            ShopManager.Instance.ShowInfo(item,this);
            SelectSlotCheck();
            if (timeSinceLastClick <= doubleClickThreshold)
            {
                ShopManager.Instance.OnAddNewItemInInventory();
            }
        }
    }
    public void SelectSlotCheck()
    {
        myFrame.color = Color.green;
        myFrame.material = highlight;
    }
    public void UnSelectSlotCheck()
    {
        myFrame.color = Color.white;
        myFrame.material = null;
    }
}
