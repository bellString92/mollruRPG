using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnlyOkButtonUI : MonoBehaviour
{
    public TextMeshProUGUI msg;
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Return))
        {
            OnOkButton();
        }
    }
    public void OnOkButton()
    {
        UIManager.Instance.CloseTopUi();
    }

}
