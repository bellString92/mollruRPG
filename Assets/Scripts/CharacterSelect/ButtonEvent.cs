using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonEvent : MonoBehaviour
{
    public UnityEvent<int> createAct;
    public UnityEvent startAct;

    public void OnCreateBtn()
    {
        createAct?.Invoke(transform.GetSiblingIndex());
    }

    public void OnStartBtn()
    {
        startAct?.Invoke();
    }

}
