using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;

public enum State { Sleep, Alert, Chase, Battle, Enraged, Death }

public class BossAI : BattleSystem
{
    public float wakeUpRange = 20;
    public float myExp = 1000;
    public State myState = State.Sleep;
    public Transform player;
    public LayerMask enemyMask;

    private int EnragedAttackCount = 0;
    private int EnragedCount = 0;
    private float distanceToPlayer;
    private bool hasWokenUp = false; // 보스가 깨어났는지 여부
    private bool isAttacking = false; // 공격이 진행 중인지 여부
    private SphereCollider sleepCollider;
    private GameObject sleepColliderObject;

    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 4f); // 바라보는 속도
    }

    private void MoveIfInChaseAnimation()
    {
        AnimatorStateInfo stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Battle") || stateInfo.IsName("AttackWait"))
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
        distanceToPlayer = Vector3.Distance(player.position, transform.position);
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
                    StartCoroutine(ChangeStateAfterIdleAnimation(State.Chase));
                }
                else if (distanceToPlayer <= this.myBattleStat.AttackRange * 1.5f && EnragedCount <= 1)
                {
                    // 추가 로직을 여기에 작성합니다.
                }
                break;

            case State.Chase:
                if (distanceToPlayer <= this.myBattleStat.AttackRange)
                {
                    if (this.myBattleStat.curHealPoint <= this.myBattleStat.maxHealPoint / 2)
                    {
                        StartCoroutine(ChangeStateAfterAnimation(State.Enraged));
                    }
                    else if (EnragedCount < 1)
                    {
                        StartCoroutine(ChangeStateAfterAnimation(State.Battle));
                    }
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
                //LookAtPlayer();
                break;
            case State.Chase:
                LookAtPlayer();
                MoveIfInChaseAnimation();
                break;

            case State.Battle:
                LookAtPlayer();
                MoveIfInChaseAnimation();
                if (!isAttacking)
                {
                    Attack();
                }
                break;

            case State.Enraged:
                LookAtPlayer();
                MoveIfInChaseAnimation();
                Enraged();
                if (!isAttacking)
                {
                    Attack();
                }
                break;

            case State.Death:
                OnDeath();
                break;
        }
    }

    bool IsIdleAnimationComplete()
    {
        AnimatorStateInfo stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);
        return !stateInfo.IsName("Idle") || stateInfo.normalizedTime >= 1f;
    }

    IEnumerator ChangeStateAfterAnimation(State newState)
    {
        AnimatorStateInfo stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);
        float animationDuration = stateInfo.length;
        yield return new WaitForSeconds(animationDuration);
        UpdateState(newState);
    }

    IEnumerator ChangeStateAfterIdleAnimation(State newState)
    {
        AnimatorStateInfo stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);

        // Idle 애니메이션이 아닐 때까지 기다립니다.
        while (!stateInfo.IsName("Idle") || stateInfo.normalizedTime < 1f)
        {
            yield return null;
            stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);
        }

        // Idle 애니메이션이 완료된 후 상태를 변경합니다.
        UpdateState(newState);
    }

    void UpdateState(State newState)
    {
        if (myState == newState) return; // 중복 상태 업데이트 방지

        myState = newState;

        switch (newState)
        {
            case State.Sleep:
                myAnim.SetBool("isSleeping", true);
                myAnim.SetBool("isMoving", false);
                if (sleepCollider != null) sleepCollider.enabled = true;
                hasWokenUp = false;
                break;

            case State.Alert:
                myAnim.SetBool("isSleeping", false);
                myAnim.SetBool("isFind", true);
                hasWokenUp = true;
                if (sleepCollider != null) sleepCollider.enabled = false;
                break;
            case State.Chase:
                myAnim.SetBool("isSleeping", false);
                myAnim.SetBool("isFind", true);
                break;

            case State.Battle:
                myAnim.SetBool("isMoving", true);
                break;

            case State.Enraged:
                myAnim.SetBool("isMoving", true);
                myAnim.SetBool("isEnraged", true);
                break;

            case State.Death:
                giveExp(myExp);
                StopAllCoroutines();  // 죽을 때 모든 코루틴 정지
                deadAct?.Invoke();
                break;
        }
    }

    public void Enraged()
    {
        if (EnragedCount < 1)
        {
            this.myBattleStat.AttackPoint += 10;
            this.myBattleStat.moveSpeed += 1;
            EnragedCount++;
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

    public void OnDeath()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    public void OnAttackAnimationEnd()
    {
        // Attack 애니메이션이 끝났을 때 호출되는 메서드
        // 애니메이션이 끝난 후 대기 상태로 돌아가도록 설정
        StartCoroutine(ResetAttack());
    }

    public void OnDamage()
    {
        if (myState != State.Chase && myState != State.Battle && myState != State.Enraged)
        {
            return; // Chase 상태 이전에는 데미지를 받지 않습니다.
        }

        Collider[] list = Physics.OverlapSphere(transform.position + transform.forward * 1.0f, 5.0f, enemyMask);
        foreach (Collider col in list)
        {
            IDamage id = col.GetComponent<IDamage>();
            if (id != null)
            {
                id.TakeDamage(this.myBattleStat.AttackPoint);
            }
        }
    }

    void Attack()
    {
        if (this.myBattleStat.curHealPoint <= 200 && EnragedAttackCount < 1)
        {
            myAnim.SetTrigger("EnragedAttack");
            EnragedAttackCount++;
        }
        else
        {
            string[] attackTriggers = { "Attack1", "Attack2", "Attack3" };
            isAttacking = true;

            string chosenAttack = attackTriggers[UnityEngine.Random.Range(0, attackTriggers.Length)];
            Debug.Log($"Setting attack trigger: {chosenAttack}");
            myAnim.SetTrigger(chosenAttack);

            // 애니메이터 상태를 강제로 확인하기 위한 디버그 로그 추가
            Debug.Log($"Current animator state before coroutine: {myAnim.GetCurrentAnimatorStateInfo(0).IsName(chosenAttack)}");

            // 공격 애니메이션이 끝날 때까지 기다립니다.
            StartCoroutine(WaitForAttackAnimation(chosenAttack));
        }
    }

    IEnumerator WaitForAttackAnimation(string attackTrigger)
    {
        Debug.Log("Starting WaitForAttackAnimation coroutine");

        AnimatorStateInfo stateInfo;
        float elapsedTime = 0f;
        float timeout = 5f; // 타임아웃 시간 설정

        while (true)
        {
            stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);
            Debug.Log($"Current state: {stateInfo.IsName(attackTrigger)}, normalizedTime: {stateInfo.normalizedTime}");

            // 상태가 일치하고 normalizedTime이 1 이상인 경우 종료
            if (stateInfo.IsName(attackTrigger) && stateInfo.normalizedTime >= 1f)
            {
                Debug.Log("Attack animation complete");
                break;
            }

            // 타임아웃 초과 시 종료
            elapsedTime += Time.deltaTime;
            if (elapsedTime > timeout)
            {
                Debug.LogError("WaitForAttackAnimation timed out");
                break;
            }

            yield return null;
        }

        // 모든 공격 트리거 초기화
        foreach (string trigger in new string[] { "Attack1", "Attack2", "Attack3" })
        {
            myAnim.ResetTrigger(trigger);
        }

        // 공격 완료 플래그 해제
        Debug.Log("Attack complete, setting isAttacking to false");
        isAttacking = false;

        // Chase 상태로 돌아가기
        UpdateState(State.Chase);
    }

    IEnumerator ResetAttack()
    {
        Debug.Log("Starting ResetAttack coroutine");

        // 공격 애니메이션이 끝난 후 Alert 상태로 돌아가기
        yield return new WaitForSeconds(0.1f); // 짧은 대기 시간 후 상태 변경
        UpdateState(State.Chase);

        Debug.Log("ResetAttack coroutine complete");
    }
}