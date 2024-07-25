using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterClick : MonoBehaviour
{
    public UnityEvent<int> selBtnAct;
    public Transform dontTouch;
    public void OnMouseDown()
    {
        if (dontTouch.GetComponentInChildren<Image>() != null) return;
        if (transform.childCount > 1)
        {
            int selNumber = transform.GetSiblingIndex();
            selBtnAct?.Invoke(selNumber);
        }
    }
}
