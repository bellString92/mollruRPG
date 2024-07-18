using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public Image icon; // 아이템 아이콘을 표시할 Image
    public ItemKind item; // 해당 슬롯에 들어있는 아이템 정보

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

    // 슬롯 클릭 이벤트 처리 함수
    public void OnClickSlot()
    {
       
        ShopManager.Instance.OnSlotClicked(item);
    }
}
