using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class ItemQuantityCheck : BaseQuanityCheck, IQuantityCheck
{
    protected override void EndEditInputField(string newText)
    {
        int newQuantity;
        if (int.TryParse(newText, out newQuantity))
        {
            newQuantity = Mathf.Clamp(newQuantity, 1, itemData.maxStack);
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
            if (number > itemData.maxStack)
            {
                inputField.text = itemData.maxStack.ToString();
                centerText.text = itemData.maxStack.ToString();
                itemData.quantity = itemData.maxStack;
            }
            else
            {
                lastValidText = newText;
                centerText.text = newText;
                itemData.quantity = number;
            }
        }
        else
        {
            inputField.text = lastValidText;
        }
    }

    protected override int GetMinQuantity() => 1;

    protected override int GetMaxQuantity() => itemData.maxStack;
}
