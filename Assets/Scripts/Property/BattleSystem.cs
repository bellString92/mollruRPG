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
    public float moveSpeed;
    public float CriticalProbability;
    public float CriticalDamage;
    public float maxHealPoint;
    public float curHealPoint;
    public float maxMagicPoint;
    public float curMagicPoint;
    public int myLevel;
    public float maxExperiencePoint;
    public float curExperiencePoint;
    public int myGold;// 최대수치 2147483647

    public float GetHpValue()
    {
        return Mathf.Clamp(curHealPoint, 0.0f, maxHealPoint) / (maxHealPoint == 0.0f ? 1.0f : maxHealPoint);
    }

    public BattleStat SetBattleStat(BattleStat myStat)
    {
        OriBattleStat ori = new OriBattleStat(myStat);
        return ori.oriBattleStat;
    }
}

public class OriBattleStat
{
    public BattleStat oriBattleStat;
    public OriBattleStat()
    {
        oriBattleStat.AttackPoint = 10.0f;
        oriBattleStat.AttackRange = 2.0f;
        oriBattleStat.AttackDelay = 2.0f;
        oriBattleStat.moveSpeed = 300.0f;
        oriBattleStat.CriticalProbability = 10.0f;
        oriBattleStat.CriticalDamage = 1.5f;
        oriBattleStat.maxHealPoint = 100.0f;
        oriBattleStat.curHealPoint = 100.0f;
        oriBattleStat.maxMagicPoint = 100.0f;
        oriBattleStat.curMagicPoint = 100.0f;
        oriBattleStat.maxExperiencePoint = 100.0f;
        oriBattleStat.myLevel = 0;
        oriBattleStat.curExperiencePoint = 0;
        oriBattleStat.myGold = 0;
    }

    public OriBattleStat(BattleStat myStat) : this()
    {
        oriBattleStat.myLevel = myStat.myLevel;
        oriBattleStat.curExperiencePoint = myStat.curExperiencePoint;
        oriBattleStat.myGold = myStat.myGold;
        LevelUpBattleStat();
    }

    private void LevelUpBattleStat()
    {
        oriBattleStat.AttackPoint = oriBattleStat.AttackPoint + ((oriBattleStat.myLevel) * 0.5f);
        oriBattleStat.AttackRange = oriBattleStat.AttackRange + ((oriBattleStat.myLevel / 5) * 0.1f);
        oriBattleStat.AttackDelay = oriBattleStat.AttackDelay - ((oriBattleStat.myLevel / 5) * 0.1f);
        oriBattleStat.moveSpeed = oriBattleStat.moveSpeed + ((oriBattleStat.myLevel) * 2.0f);
        oriBattleStat.CriticalProbability = oriBattleStat.CriticalProbability + ((oriBattleStat.myLevel) * 0.5f);
        oriBattleStat.CriticalDamage = oriBattleStat.CriticalDamage + ((oriBattleStat.myLevel) * 0.1f);
        oriBattleStat.maxHealPoint = oriBattleStat.maxHealPoint + ((oriBattleStat.myLevel) * 10.0f);
        oriBattleStat.maxMagicPoint = oriBattleStat.maxMagicPoint + ((oriBattleStat.myLevel) * 10.0f);
        oriBattleStat.maxExperiencePoint = oriBattleStat.maxExperiencePoint + ((oriBattleStat.myLevel) * 10.0f);
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
    public virtual void TakeDamage(float dmg)
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

