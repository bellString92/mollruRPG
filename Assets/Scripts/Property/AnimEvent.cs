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
    public UnityEvent<float> skill_AttackAct;
    public UnityEvent<Skill> onComboAct;
    public UnityEvent<Skill> offComboAct;



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

    public void OnSkill_Attack(float v)
    {
        skill_AttackAct?.Invoke(v);
    }

    public void OnCombo(Skill v)
    {
        onComboAct?.Invoke(v);
    }

    public void OffCombo(Skill v)
    {
        offComboAct?.Invoke(v);
    }
}
