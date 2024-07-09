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
    public UnityEvent skillDamageAct;
    public UnityEvent SkillMovementAct;
    public UnityEvent skillStartAct;
    public UnityEvent skillEndAct;
    public UnityEvent comboCheckStartAct;
    public UnityEvent comboCheckEndAct;
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

    public void OnSkilldamage()
    {
        skillDamageAct?.Invoke();
    }
    public void OnSkillMovement()
    {
        SkillMovementAct?.Invoke();
    }

    public void OnSkillStart()
    {
        skillStartAct?.Invoke();
    }

    public void OnSkillEnd()
    {
        skillEndAct?.Invoke();
    }

    public void ComboCheckStart()
    {
        comboCheckStartAct?.Invoke();
    }

    public void ComboCheckEnd()
    {
        comboCheckEndAct?.Invoke();
    }
}
