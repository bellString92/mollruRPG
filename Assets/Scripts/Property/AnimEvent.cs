using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimEvent : MonoBehaviour
{
    public UnityEvent damageAct;
    public UnityEvent deadAct;
    public UnityEvent moveAct;
    public UnityEvent stopAct;
    public UnityEvent resetAct;
    public UnityEvent ComboAct;
    public UnityEvent attackAct;
    public UnityEvent skill_1Act;
    public UnityEvent skill_2Act;
    public UnityEvent skill_3Act;
    public UnityEvent skill_4Act;
    public UnityEvent skill_F1Act;
    public UnityEvent skill_F2Act;
    public UnityEvent skill_QAct;
    public UnityEvent skill_SAct;
    public UnityEvent skill_SPAct;
    public UnityEvent skill_TabAct;


    public void OnDamage()
    {
        damageAct?.Invoke();
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

    public void OnReSetCheck()
    {
        resetAct?.Invoke();
    }
    
    public void OnComboCheck()
    {
        ComboAct?.Invoke();
    }

    public void OnAttack()
    {
        attackAct?.Invoke();
    }

    public void OnSkill_1Act()
    {
        skill_1Act?.Invoke();
    }

    public void OnSkill_2Act()
    {
        skill_2Act?.Invoke();
    }

    public void OnSkill_3Act()
    {
        skill_3Act?.Invoke();
    }

    public void OnSkill_4Act()
    {
        skill_4Act?.Invoke();
    }

    public void OnSkill_F1Act()
    {
        skill_F1Act?.Invoke();
    }

    public void OnSkill_F2Act()
    {
        skill_F2Act?.Invoke();
    }

    public void OnSkill_QAct()
    {
        skill_QAct?.Invoke();
    }

    public void OnSkill_SAct()
    {
        skill_SAct?.Invoke();
    }

    public void OnSkill_SPAct()
    {
        skill_SPAct?.Invoke();
    }

    public void OnSkill_TabAct()
    {
        skill_TabAct?.Invoke();
    }

}
