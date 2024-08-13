using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropPoketSlot : MonoBehaviour
{
    public ItemKind setitem;
    public Image icon;
    public TextMeshProUGUI itemQuanity;
    // Start is called before the first frame update
    private void OnEnable()
    {
        if (setitem != null)
        {
            icon.sprite = setitem.itemIcon;
            UpdateQuanity();
        }
    }
    public void UpdateQuanity()
    {
        if(setitem == null)
        {
            itemQuanity.text = "";
        }
        else if (setitem.quantity > 1)
        {
            itemQuanity.text = setitem.quantity.ToString();
        }
        else
        {
            itemQuanity.text = "";
        }
    }
    public void AddQuantity(int amount)
    {
        if (setitem != null)
        {
            setitem.quantity += amount;  // 수량을 추가
            UpdateQuanity();  // 추가된 후 수량 업데이트
        }
    }
}
