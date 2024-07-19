using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimEvent : MonoBehaviour
{
    public UnityEvent attackAct;
    public UnityEvent deadAct;
    public UnityEvent moveAct;
    public UnityEvent stopAct;

    public void OnAttack()
    {
        attackAct?.Invoke();
    }

    public void OnDead()
    {
        deadAct?.Invoke();
    }

    public void OnMove()
    {
        moveAct?.Invoke();
    }

    public void OnStop()
    {
        stopAct?.Invoke();
    }
}
