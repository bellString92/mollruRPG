using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;

public enum State { Sleep, Alert, Chase, Battle, Enraged, Death, Attacking }

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

    public GameObject attackRangePrefab;
    private GameObject currentAttackRange;

    private void LookAtPlayer()
    {
        AnimatorStateInfo stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);

        if (distanceToPlayer > this.myBattleStat.AttackRange / 2)
        {
            if (player == null) return;

            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 1.5f); // 바라보는 속도
        }
    }

    private void MoveIfInChaseAnimation()
    {
        AnimatorStateInfo stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Battle") || stateInfo.IsName("Chase"))
        {
            MoveTowardsPlayer();
        }
    }

    public void MoveTowardsPlayer()
    {
        AnimatorStateInfo stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);
        if (distanceToPlayer <= this.myBattleStat.AttackRange / 2)
        {
            //myAnim.SetBool("isMoving", false);
            return;
        }
        else if (distanceToPlayer > this.myBattleStat.AttackRange / 2)
        {
            if (player == null) return;

            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            myAnim.SetBool("isMoving", true);
        }
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
        //Debug.Log(distanceToPlayer);
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
                    // 추가 동작을 여기서 구현
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
                // LookAtPlayer(); // Alert 상태에서는 플레이어를 바라보지 않음
                break;

            case State.Chase:
                LookAtPlayer();
                MoveIfInChaseAnimation();
                break;

            case State.Battle:
                if (distanceToPlayer <= this.myBattleStat.AttackRange / 2)
                {
                    if (!isAttacking)
                    {
                        Attack();
                    }
                }
                else
                {
                    LookAtPlayer();
                    MoveIfInChaseAnimation();
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

            case State.Attacking:
                // 공격 중일 때는 회전하지 않음
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
                myAnim.SetBool("isAttacking", false); // Attacking 상태 해제
                break;

            case State.Battle:
                myAnim.SetBool("isMoving", true);
                myAnim.SetBool("isAttacking", false); // Attacking 상태 해제
                break;

            case State.Enraged:
                myAnim.SetBool("isMoving", true);
                myAnim.SetBool("isEnraged", true);
                myAnim.SetBool("isAttacking", false); // Attacking 상태 해제
                break;

            case State.Death:
                giveExp(myExp);
                StopAllCoroutines();  // 죽을 때 모든 코루틴 정지
                deadAct?.Invoke();
                break;

            case State.Attacking:
                myAnim.SetBool("isAttacking", true); // Attacking 상태 활성화
                myAnim.SetBool("isMoving", false); // 이동 비활성화
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
        // 애니메이션이 끝난 후 Chase 상태로 돌아가도록 설정
        StartCoroutine(ResetAttack());
    }

    public void OnDamage()
    {
        if (myState != State.Chase && myState != State.Battle && myState != State.Enraged)
        {
            return;
        }

        Collider[] list = Physics.OverlapSphere(transform.position + transform.forward * 6.0f, 9.0f, enemyMask);

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

            // 상태를 Attacking으로 변경
            UpdateState(State.Attacking);

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
            if (stateInfo.IsName(attackTrigger) && stateInfo.normalizedTime >= 1f)
            {
                Debug.Log("Attack animation complete");
                break;
            }

            elapsedTime += Time.deltaTime;
            if (elapsedTime > timeout)
            {
                //Debug.LogError("WaitForAttackAnimation timed out");
                break;
            }

            yield return null;
        }

        // 모든 공격 트리거 초기화
        foreach (string trigger in new string[] { "Attack1", "Attack2", "Attack3" })
        {
            myAnim.ResetTrigger(trigger);
        }

        // 공격 완료 후 상태를 변경
        isAttacking = false;
        UpdateState(State.Battle);

        // 대기 코루틴 시작
        StartCoroutine(WaitAfterAttack());
    }

    IEnumerator WaitAfterAttack()
    {
        // 1초 동안 대기
        yield return new WaitForSeconds(1.0f);

        // Chase 상태로 돌아가기
        UpdateState(State.Battle);
    }

    IEnumerator ResetAttack()
    {
        Debug.Log("Starting ResetAttack coroutine");

        // 공격 애니메이션이 끝난 후 Chase 상태로 돌아가기
        yield return new WaitForSeconds(0.1f); // 짧은 대기 시간 후 상태 변경
        UpdateState(State.Chase);

        Debug.Log("ResetAttack coroutine complete");
    }
    public void ShowAttackRange()
    {
        if (attackRangePrefab != null)
        {
            currentAttackRange = Instantiate(attackRangePrefab, transform.position + transform.forward * 6.0f, Quaternion.identity);
        }
    }

    public void HideAttackRange()
    {
        if (currentAttackRange != null)
        {
            Destroy(currentAttackRange);
        }
    }
}