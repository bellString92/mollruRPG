using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public Image icon; // ������ �������� ǥ���� Image
    public ItemKind item; // �ش� ���Կ� ����ִ� ������ ����

    // ���� �ʱ�ȭ �Լ�
    public void InitializeSlot(ItemKind newItem)
    {
        item = newItem;
        if (item != null) 
        {
            icon.sprite = item.itemIcon;
            icon.enabled = true;
        }
    }

    // ���� Ŭ�� �̺�Ʈ ó�� �Լ�
    public void OnClickSlot()
    {
       
        ShopManager.Instance.OnSlotClicked(item);
    }
}
