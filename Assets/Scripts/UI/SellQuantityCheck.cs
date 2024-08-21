using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class SellQuantityCheck : BaseQuanityCheck, IQuantityCheck
{
    protected override void EndEditInputField(string newText)
    {
        int newQuantity;
        if (int.TryParse(newText, out newQuantity))
        {
            newQuantity = Mathf.Clamp(newQuantity, 0, itemData.quantity);
            UpdateQuantity(newQuantity);
        }
        else
        {
            inputField.text = lastValidText;
            centerText.text = lastValidText;
        }

        inputField.gameObject.SetActive(false);
        centerText.gameObject.SetActive(true);
    }

    protected override void ValidateInput(string newText)
    {
        int number;
        if (string.IsNullOrEmpty(newText))
        {
            inputField.text = "";
        }
        else if (int.TryParse(newText, out number))
        {
            if (number > itemData.quantity)
            {
                inputField.text = itemData.quantity.ToString();
                centerText.text = itemData.quantity.ToString();
            }
            else
            {
                lastValidText = newText;
                centerText.text = newText;
            }
        }
        else
        {
            inputField.text = lastValidText;
        }
    }

    protected override int GetMinQuantity() => 0;

    protected override int GetMaxQuantity() => itemData.quantity;
}