using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveItemInfo : MonoBehaviour
{
    public ItemKind itemKind;
    private TextMeshProUGUI quantityText; // 수량을 표시하는 텍스트
    private int lastQuantity; // 마지막으로 표시된 수량

    private void Start()
    {
        quantityText = GetComponentInChildren<TextMeshProUGUI>();
        if (quantityText != null)
        {
            lastQuantity = itemKind.quantity;
            UpdateQuantityText(); // 초기 수량 업데이트
        }
    }

    private void Update()
    {
        // 수량이 변경되었는지 확인하고 UI를 업데이트합니다.
        if (itemKind.quantity != lastQuantity)
        {
            UpdateQuantityText();
            lastQuantity = itemKind.quantity;
        }
    }

    private void UpdateQuantityText()
    {
        if (quantityText != null)
        {
            quantityText.text = itemKind.quantity.ToString();
        }
    }
}
