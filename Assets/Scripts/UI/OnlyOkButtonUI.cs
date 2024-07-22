using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyOkButtonUI : MonoBehaviour
{
    public void OnOkButton()
    {
        UIManager.Instance.CloseTopUi();
    }

}
