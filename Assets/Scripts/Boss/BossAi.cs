using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using Unity.Mathematics;


public enum State { Sleep, Alert, Battle, Enraged, Death }

public class BossAI : BattleSystem
{
    public float wakeUpRange = 20;
    public float myExp = 1000;
    public State myState = State.Sleep;
    public Transform player;
    public LayerMask enemyMask;

    private float distanceToPlayer;
    private bool hasWokenUp = false;
    private SphereCollider sleepCollider;
    private GameObject sleepColliderObject;

    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // 바라보는 속도
    }

    private void MoveIfInChaseAnimation()
    {
        AnimatorStateInfo stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Battle"))
        {
            MoveTowardsPlayer();
        }
    }
    public void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        myAnim.SetBool("isMoving", true);
    }

    void Awake()
    {
        sleepColliderObject = transform.Find("SleepCollider")?.gameObject;
        if (sleepColliderObject != null)
        {
            sleepCollider = sleepColliderObject.GetComponent<SphereCollider>();
        }
    }

    void Start()
    {
        UpdateState(State.Sleep);

        this.myBattleStat.curHealPoint = this.myBattleStat.maxHealPoint;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        //Debug.Log($"Distance to Player: {distanceToPlayer}");

        UpdateStateBasedOnDistance(distanceToPlayer);
        PerformActionBasedOnState();
    }

    void UpdateStateBasedOnDistance(float distanceToPlayer)
    {
        switch (myState)
        {
            case State.Sleep:
                if (!hasWokenUp && distanceToPlayer < wakeUpRange)
                {
                    StartCoroutine(ChangeStateAfterAnimation(State.Alert));
                }
                break;

            case State.Alert:
                if (distanceToPlayer <= this.myBattleStat.AttackRange)
                {
                    StartCoroutine(ChangeStateAfterAnimation(State.Battle));
                }
                break;

            case State.Battle:
                if (this.myBattleStat.curHealPoint <= this.myBattleStat.maxHealPoint / 2 && myState != State.Enraged)
                {
                    StartCoroutine(ChangeStateAfterAnimation(State.Enraged));
                }
                break;

            case State.Enraged:
                if (this.myBattleStat.curHealPoint <= 0)
                {
                    StartCoroutine(ChangeStateAfterAnimation(State.Death));
                }
                break;
        }
    }

    void PerformActionBasedOnState()
    {
        switch (myState)
        {
            case State.Sleep:

                break;

            case State.Alert:
                LookAtPlayer();
                break;

            case State.Battle:
                LookAtPlayer();
                MoveIfInChaseAnimation();
                StartCoroutine(BossAttack());
                break;

            case State.Enraged:
                LookAtPlayer();
                MoveIfInChaseAnimation();
                StartCoroutine(BossAttack());
                break;

            case State.Death:

                break;
        }
    }

    IEnumerator ChangeStateAfterAnimation(State newState)
    {
        AnimatorStateInfo stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);
        float animationDuration = stateInfo.length;

        yield return new WaitForSeconds(animationDuration);

        UpdateState(newState);
    }

    void UpdateState(State newState)
    {
        if (myState == newState) return;  // 중복 상태 업데이트 방지

        myState = newState;

        switch (newState)
        {
            case State.Sleep:
                myAnim.SetBool("isSleeping", true);
                myAnim.SetBool("isMoving", false);
                if (sleepCollider != null) sleepCollider.enabled = true;
                hasWokenUp = false; // Sleep 상태로 전환할 때 hasWokenUp을 false로 초기화
                break;

            case State.Alert:
                myAnim.SetBool("isSleeping", false);
                myAnim.SetBool("isFind", true);
                hasWokenUp = true;
                if (sleepCollider != null) sleepCollider.enabled = false;
                break;

            case State.Battle:
                myAnim.SetBool("isMoving", true);
                myAnim.SetTrigger("OnAttack");
                // Battle 상태에서 추가 로직
                break;

            case State.Enraged:
                myAnim.SetBool("isMoving", true);
                myAnim.SetBool("isEnraged", true);
                //moveSpeed *= 1.5f;
                break;

            case State.Death:
                myAnim.SetTrigger("die");
                StopAllCoroutines();  // 죽을 때 모든 코루틴 정지
                break;
        }
    }

    public void giveExp(float exp)
    {
        if (myTarget == null) return;
        OnGiveExp(myExp);
    }

    public void OnSleep()
    {
        myTarget = null;
        UpdateState(State.Sleep);
    }

    public void OnBattle(Transform target)
    {
        myTarget = target;
        UpdateState(State.Battle);
    }

    public void OnAlert()
    {
        myTarget = null;
        UpdateState(State.Alert);
    }
    public void OnDamage()
    {
        Collider[] list = Physics.OverlapSphere(transform.position + transform.forward * 5.0f, 5.0f, enemyMask);
        foreach (Collider col in list)
        {
            IDamage id = col.GetComponent<IDamage>();
            if (id != null)
            {
                id.TakeDamage(this.myBattleStat.AttackPoint);
            }
        }
    }



    IEnumerator BossAttack()
    {
        string[] attackTriggers = { "Attack1", "Attack2", "Attack3" };

        while (true)
        {
            // 랜덤으로 하나의 트리거 선택
            string chosenAttack = attackTriggers[UnityEngine.Random.Range(0, attackTriggers.Length)];
            myAnim.SetTrigger(chosenAttack);

            // 선택한 트리거에 해당하는 애니메이션 길이 기다리기
            AnimatorStateInfo stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);
            float attackAnimationDuration = stateInfo.length;
            yield return new WaitForSeconds(attackAnimationDuration);

            // 애니메이션 종료 후 3초 기다리기
            yield return new WaitForSeconds(3.0f);
        }
    }
}