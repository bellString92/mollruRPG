using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyOkButtonUI : MonoBehaviour
{
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
