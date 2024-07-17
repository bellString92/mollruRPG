using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

interface IDamage
{
    void TakeDamage(float dmg);
}

interface ILive
{
    bool IsLive { get; }
}

interface IBattle : IDamage, ILive
{

}

[System.Serializable]
public struct BattleStat
{
    public float AttackPoint;
    public float AttackRange;
    public float AttackDelay;
    public float maxHealPoint;
    public float curHealPoint;
    public float maxMagicPoint;
    public float curMagicPoint;
    public int myLvevel;
    public float maxExperiencePoint;
    public float curExperiencePoint;

    public float GetHpValue()
    {
        return Mathf.Clamp(curHealPoint, 0.0f, maxHealPoint) / (maxHealPoint == 0.0f ? 1.0f : maxHealPoint);
    }
}

public class BattleSystem : AIMovement, IBattle
{
    protected UnityAction<float> changeHpAct;
    public UnityEvent deadAct;
    public Transform myTarget;
    public BattleStat myBattleStat;
    protected float playTime;

    public bool IsLive
    {
        get => myBattleStat.curHealPoint > 0.0f;
    }

    protected virtual void OnDead()
    {

    }
    public void TakeDamage(float dmg)
    {
        myBattleStat.curHealPoint -= dmg;
        changeHpAct?.Invoke(myBattleStat.GetHpValue());
        myAnim.ResetTrigger("OnDamage");
        if (myBattleStat.curHealPoint <= 0.0f)
        {
            myAnim.SetTrigger("OnDead");
            OnDead();
        }
        else
        {
            myAnim.SetTrigger("OnDamage");
        }
    }

    protected void OnAttack()
    {
        if (myAnim.GetBool("IsAttacking") == false)
        {            
            if (myTarget.GetComponent<ILive>().IsLive && playTime >= myBattleStat.AttackDelay)
            {
                playTime = 0.0f;
                myAnim.SetTrigger("OnAttack");
            }
        }
    }

    protected void OnGiveExp(float exp)
    {
        if (!myAnim.GetBool("IsDead"))
        {
            myAnim.SetBool("IsDead", true);
            myTarget.GetComponent<Player>().myStat.curExperiencePoint += exp;
        }
    }

    protected void BattleUpdate()
    {
        if (myAnim.GetBool("IsAttacking") == false)
        {
            playTime += Time.deltaTime;
        }
    }

    public void OnAttackTarget()
    {
        if (Vector3.Distance(myTarget.position, transform.position) <= myBattleStat.AttackRange)
        {
            myTarget.GetComponent<IDamage>()?.TakeDamage(myBattleStat.AttackPoint);
        }
    }
}
