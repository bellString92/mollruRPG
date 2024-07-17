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
    public UnityEvent takeExpAct;
    public UnityEvent skillDamageAct;
    public UnityEvent skillCheckStartAct;
    public UnityEvent skillCheckEndAct;
    public UnityEvent AttackCheckStartAct;
    public UnityEvent AttackCheckEndAct;
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

    public void OntakeExp()
    {
        takeExpAct?.Invoke();
    }

    public void OnSkilldamage()
    {
        skillDamageAct?.Invoke();
    }
 

    public void OnSkillCheckStart()
    {
        skillCheckStartAct?.Invoke();
    }

    public void OnSkillCheckEnd()
    {
        skillCheckEndAct?.Invoke();
    }

    public void OnAttackCheckStart()
    {
        AttackCheckStartAct?.Invoke();
    }

    public void OnAttackCheckEnd()
    {
        AttackCheckEndAct?.Invoke();
    }
}
