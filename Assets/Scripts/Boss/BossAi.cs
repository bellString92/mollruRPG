using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using Unity.VisualScripting;

public enum State { Sleep, Alert, Battle, Enraged, Death,}

public class BossAI : BattleSystem
{
    public float wakeUpRange = 20;
    public float myExp = 1000;
    public State myState = State.Sleep;
    public Transform player;
    public LayerMask enemyMask;
    public Transform head;

    private float wrpNum; // 플레이어와 거리에 따라 증가하는 수치
    private float AttackdistanceToPlayer = 0.0f;
    private int EnragedAttackCount = 0;
    private int EnragedCount = 0;
    private float distanceToPlayer;
    private bool hasWokenUp = false; // 보스가 깨어났는지 여부
    private float BossAttackTriggerDelay;

    private SphereCollider sleepCollider;
    private GameObject sleepColliderObject;

    public GameObject attackRangePrefab1;
    public GameObject attackRangePrefab2;
    public GameObject attackRangePrefab3;

    private GameObject currentAttackRange;

    private void LookAtPlayer()
    {
        AnimatorStateInfo stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);
        
        if (/*(distanceToPlayer > this.myBattleStat.AttackRange / 2) &&*/ stateInfo.IsName("Battle"))
        {
            if (player == null) return;

            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * (1.5f + (wrpNum * 50))); // 바라보는 속도
        }
    }

    void AttackLookPlayer()
    {
        float AttackLookTime = 0.0f;
        if(AttackLookTime < 1.5f)
        {
            AttackLookTime += Time.deltaTime;
            LookAtPlayer();
        }
        else if(AttackLookTime > 1.5f)
        {
            AttackLookTime = 0.0f;
            return;
        }
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
            transform.position += direction * Time.deltaTime * (moveSpeed + (wrpNum * 6)) ;
            //myAnim.SetBool("isMoving", true);
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
        //Debug.Log(distanceToPlayer);

        AttackdistanceToPlayer = Vector3.Distance(player.position, head.transform.position);
        Debug.Log(AttackdistanceToPlayer);
 

        if(AttackdistanceToPlayer >= this.myBattleStat.AttackRange / 3)
        {
            StopCoroutine(Attack());
        }
        UpdateStateBasedOnDistance(distanceToPlayer);
        //Debug.Log(distanceToPlayer);
        PerformActionBasedOnState();
        CheckWalkOrRun();
    }

    void CheckWalkOrRun()
    {
        AnimatorStateInfo stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);
        if ((distanceToPlayer - 10) > 0 && stateInfo.IsName("Battle"))
        {
            if (wrpNum <= 0.5f)
            {
                wrpNum = (distanceToPlayer - 10) / 30;

                if (wrpNum < 0) wrpNum = 0;
                else if (wrpNum > 0.5f) wrpNum = 0.5f;

                myAnim.SetFloat("WRP", wrpNum);
            }
        }
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
                    StartCoroutine(ChangeStateAfterIdleAnimation(State.Battle));
                }
                else if (distanceToPlayer <= this.myBattleStat.AttackRange * 1.5f && EnragedCount <= 1)
                {
                    // 추가 동작을 여기서 구현
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
                myAnim.SetBool("isMoving", false);
                break;

            case State.Battle:
                BossAttackTriggerDelay += Time.deltaTime;
                myAnim.SetBool("isMoving", true);
                if (AttackdistanceToPlayer <= this.myBattleStat.AttackRange / 3 && BossAttackTriggerDelay > 1.5f)
                {
                    BossAttackTriggerDelay = 0.0f;
                    StartCoroutine(Attack());
                }
                else
                {
                    StopCoroutine(Attack());
                    LookAtPlayer();
                    MoveIfInChaseAnimation();
                }
                break;

            case State.Enraged:
                BossAttackTriggerDelay += Time.deltaTime;
                myAnim.SetBool("isMoving", true);
                if (AttackdistanceToPlayer <= this.myBattleStat.AttackRange / 3 && BossAttackTriggerDelay > 1.5f)
                {
                    BossAttackTriggerDelay = 0.0f;
                    StartCoroutine(Attack());
                }
                else
                {
                    StopCoroutine(Attack());
                    LookAtPlayer();
                    MoveIfInChaseAnimation();
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
        while (!stateInfo.IsName("Scream") || !stateInfo.IsName("Idle") || stateInfo.normalizedTime < 1f)
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
        // 애니메이션이 끝난 후 Battle 상태로 돌아가도록 설정
        StartCoroutine(ResetAttack());
    }

    /*public void OnDamage()
    {
        Collider[] list = Physics.OverlapSphere(transform.position + transform.forward * 6.0f, 9.0f, enemyMask);

        foreach (Collider col in list)
        {
            IDamage id = col.GetComponent<IDamage>();
            if (id != null)
            {
                id.TakeDamage(this.myBattleStat.AttackPoint);
            }
        }
    }*/

    public void OnDamage(string attackType)
    {
        Collider[] list = null;

        // 공격 종류에 따라 범위와 크기, 데미지를 다르게 설정
        switch (attackType)
        {
            case "Attack1": // 상자 모양
                {
                    Vector3 point1 = transform.position + transform.forward * 2.0f; // 캡슐의 첫 번째 끝점
                    Vector3 point2 = transform.position + transform.forward * 6.0f; // 캡슐의 두 번째 끝점
                    float radius = 3.0f;
                    int damage = Mathf.RoundToInt(this.myBattleStat.AttackPoint - 5);

                    list = Physics.OverlapCapsule(point1, point2, radius, enemyMask);
                    ApplyDamage(list, damage);
                    break;
                }
            case "Attack2": // 구 모양
                {
                    Vector3 offset = transform.forward * 6.0f;
                    float radius = 8.0f;
                    int damage = Mathf.RoundToInt(this.myBattleStat.AttackPoint + 10);

                    list = Physics.OverlapSphere(transform.position + offset, radius, enemyMask);
                    ApplyDamage(list, damage);
                    break;
                }
            case "Attack3": // 상자 모양
                {
                    Vector3 center = transform.position + transform.forward * 7.0f;
                    Vector3 halfExtents = new Vector3(6.0f, 4.0f, 8.0f); // 상자의 반쪽 크기
                    Quaternion orientation = Quaternion.identity;
                    int damage = Mathf.RoundToInt(this.myBattleStat.AttackPoint);

                    list = Physics.OverlapBox(center, halfExtents, orientation, enemyMask);
                    ApplyDamage(list, damage);
                    break;
                }
        }
    }

    private void ApplyDamage(Collider[] targets, int damage)
    {
        foreach (Collider col in targets)
        {
            IDamage id = col.GetComponent<IDamage>();
            if (id != null)
            {
                id.TakeDamage(damage);  // 설정된 데미지로 공격
            }
        }
    }


    IEnumerator Attack()
    {
        wrpNum = 0;
        AttackLookPlayer();
        if (AttackdistanceToPlayer <= this.myBattleStat.AttackRange / 3)
        {
            if (this.myBattleStat.curHealPoint <= 200 && EnragedAttackCount < 1)
            {
                myAnim.SetTrigger("EnragedAttack");
                EnragedAttackCount++;
            }
            else
            {
                string[] attackTriggers = { "Attack1", "Attack2", "Attack3" };
                bool attackTriggered = false;

                while (!attackTriggered)
                {

                    // 다른 공격 트리거가 활성화되어 있는지 확인
                    bool isAnyAttackActive = myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") ||
                                             myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack2") ||
                                             myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack3");
                    if (!isAnyAttackActive)
                    {
                        // 트리거 중 하나를 랜덤하게 선택
                        string chosenAttack = attackTriggers[UnityEngine.Random.Range(0, attackTriggers.Length)];
                        Debug.Log($"Setting attack trigger: {chosenAttack}");
                        myAnim.SetTrigger(chosenAttack);

                        if (AttackdistanceToPlayer > 6.0f)
                        {
                            myAnim.ResetTrigger(chosenAttack);
                        }

                        // 공격 트리거가 성공적으로 설정되었음을 표시
                        attackTriggered = true;

                        // 공격 애니메이션이 끝날 때까지 기다립니다.
                        StartCoroutine(WaitForAttackAnimation(chosenAttack));
                        break;
                    }
                    else
                    {
                        // 다른 트리거가 활성화되어 있다면 2초 대기
                        yield return new WaitForSeconds(2.0f);
                    }
                }
            }
        }
        yield break;
    }

    IEnumerator WaitForAttackAnimation(string attackTrigger)
    {
        Debug.Log("Starting WaitForAttackAnimation coroutine");

        AnimatorStateInfo stateInfo;
        float elapsedTime = 0f;
        float timeout = 2.0f; // 타임아웃 시간 설정

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

        // 대기 코루틴 시작
        StartCoroutine(WaitAfterAttack());
    }

    IEnumerator WaitAfterAttack()
    {
        // 1초 동안 대기
        yield return new WaitForSeconds(1.0f);

    }

    IEnumerator ResetAttack()
    {
        //Debug.Log("Starting ResetAttack coroutine");
        // 공격 애니메이션이 끝난 후 Battle 상태로 돌아가기
        yield return new WaitForSeconds(1.0f); // 짧은 대기 시간 후 상태 변경
        //Debug.Log("ResetAttack coroutine complete");
    }
    public void ShowAttackRange(string attackType)
    {
        // 공격 타입에 따라 프리팹 선택
        GameObject prefabToUse = null;

        switch (attackType)
        {
            case "Attack1":
                prefabToUse = attackRangePrefab1;
                break;
            case "Attack2":
                prefabToUse = attackRangePrefab2;
                break;
            case "Attack3":
                prefabToUse = attackRangePrefab3;
                break;
        }
        if (prefabToUse != null)
        {
            // 공격 범위 프리팹 인스턴스화
            currentAttackRange = Instantiate(prefabToUse,
                transform.position + transform.forward * 7.0f, Quaternion.identity);

            // 몬스터의 y 축 회전과 동일하게 공격 범위의 y 축 회전 설정
            Vector3 rotation = transform.rotation.eulerAngles;
            currentAttackRange.transform.rotation = Quaternion.Euler(0, rotation.y, 0);
        }
    }


    public void HideAttackRange()
    {
        if (currentAttackRange != null)
        {
            Destroy(currentAttackRange);
        }
    }

    public void OnAttackStart(string attackType)
    {
        ShowAttackRange(attackType);
    }

    // 애니메이션 이벤트로 공격 범위를 숨김
    public void OnAttackEnd()
    {
        HideAttackRange();
    }
}