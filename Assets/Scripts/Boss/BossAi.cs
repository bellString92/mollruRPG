using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;
// 애니메이션이 끝나고 나서 상태를 변경하게 수정해야함
// Attack 상태를 구체화 (Chase상태에서 Attack상태로 갈때 너무 짧게 돌아옴)

public enum CurrentState { Sleep, Alert, Chase, Attack, Enraged, Dead }

public class BossAI : MonoBehaviour
{
    [Header("Game Settings")]
    public float moveSpeed;
    public float attackRange;
    public float wakeUpRange;
    public float attackStateRange;
    public float maxHP;
    public float currentHealth;
    public CurrentState currentState;
    public Transform player;

    
    private Animator animator;
    private Collider mainCollider;
    private SphereCollider sleepCollider;
    private GameObject sleepColliderObject;
    private bool hasWokenUp = false;
    private float timer;
    private float delaytimer;
    private NavMeshAgent agent;
    private bool isChaseing = false; 
    private bool isAttacking = false;
    private bool isFirstAttack = true;

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
        if (sleepCollider == null)
        {
            //Debug.LogError("SleepCollider not found. Please ensure there is a child object named 'SleepCollider' with a SphereCollider attached.");
            return;
        }

        currentHealth = maxHP;

        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        { 
            return;
        }

        mainCollider = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();

        currentState = CurrentState.Sleep;
        UpdateState(CurrentState.Sleep);

        timer = 0.0f;
        delaytimer = 4.0f;
    }

    void Update()
    {

        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // 항상 플레이어를 바라보게
        if (isChaseing == true && !isAttacking) // 공격중에는 플레이어를 바라보지않게
        {
            LookAtPlayer();
        }

        switch (currentState)
        {
            case CurrentState.Sleep:
                if (!hasWokenUp && distanceToPlayer < wakeUpRange)
                {
                    UpdateState(CurrentState.Alert);
                }
                break;

            case CurrentState.Alert:
                if (distanceToPlayer <= attackStateRange)
                {
                    UpdateState(CurrentState.Chase);
                }
                break;

            case CurrentState.Chase:
                if (distanceToPlayer <= attackRange)
                {
                    UpdateState(CurrentState.Attack);
                    isChaseing = false;
                }
                else
                {
                    MoveIfInChaseAnimation();
                }
                break;

            case CurrentState.Attack:
                if (distanceToPlayer <= attackRange)
                {
                    StartCoroutine(Attack());
                }
                else
                {
                    UpdateState(CurrentState.Chase);
                }
                break;

            case CurrentState.Enraged:
                if (distanceToPlayer <= attackRange)
                {
                    StartCoroutine(Attack());
                }
                else
                {
                    
                }
                break;

            case CurrentState.Dead:
                break;
        }
    }

    private void LookAtPlayer()
    {
        if (player == null) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 공격 애니메이션 중에는 회전하지 않도록 설정
        if (stateInfo.IsName("attack1") || stateInfo.IsName("attack2") || stateInfo.IsName("attack3"))
        {
            return;
        }

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // 바라보는 속도
    }

    private void MoveIfInChaseAnimation()
    {
        isChaseing = true;
        isAttacking = false;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Chase"))
        {
            MoveTowardsPlayer();
        }
    }

    public void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        animator.SetBool("isMoving", true);
    }

    public IEnumerator Attack()
    {
        isAttacking = true; 
        animator.SetBool("isMoving", false);

        int attackIndex = Random.Range(0, 3);
        timer += Time.deltaTime;

        if (timer > delaytimer)
        {
            switch (attackIndex)
            {
                case 0:
                    animator.SetTrigger("attack1");
                    break;
                case 1:
                    animator.SetTrigger("attack2");
                    break;
                case 2:
                    animator.SetTrigger("attack3");
                    break;
            }
            timer = 0;
        }

        // 공격 애니메이션이 끝날때가지 기다림
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float attackAnimationDuration = stateInfo.length;
        yield return new WaitForSeconds(attackAnimationDuration);

        isAttacking = false; 
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            UpdateState(CurrentState.Dead);
        }
        else if (currentHealth <= maxHP / 2 && currentState != CurrentState.Enraged)
        {
            UpdateState(CurrentState.Enraged);
        }
    }

    public void UpdateState(CurrentState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case CurrentState.Sleep:
                animator.SetBool("isSleeping", true);
                animator.SetBool("isMoving", false);
                if (sleepCollider != null) sleepCollider.enabled = true;
                break;

            case CurrentState.Alert:
                animator.SetBool("isSleeping", false);
                animator.SetBool("isFind", true);
                hasWokenUp = true;
                if (sleepCollider != null) sleepCollider.enabled = false;
                break;

            case CurrentState.Chase:
                animator.SetBool("isMoving", true);
                animator.SetBool("isSleeping", false);
                break;

            case CurrentState.Attack:
                if (isFirstAttack)
                {
                    StartCoroutine(Attack(firstAttack: true));
                }
                else
                {
                    StartCoroutine(Attack(firstAttack: false));
                }
                animator.SetBool("isMoving", false);
                break;

            case CurrentState.Enraged:
                animator.SetBool("isEnraged", true);
                moveSpeed *= 1.5f;
                break;

            case CurrentState.Dead:
                animator.SetTrigger("die");
                break;
        }
    }

    public IEnumerator Attack(bool firstAttack)
    {
        isAttacking = true;
        animator.SetBool("isMoving", false);

        if (firstAttack)
        {
            isFirstAttack = false;
        }
        else
        {
            timer += Time.deltaTime;
            if (timer < delaytimer)
            {
                yield return new WaitForSeconds(delaytimer - timer);
            }
            timer = 0;
        }

        int attackIndex = Random.Range(0, 3);
        switch (attackIndex)
        {
            case 0:
                animator.SetTrigger("attack1");
                break;
            case 1:
                animator.SetTrigger("attack2");
                break;
            case 2:
                animator.SetTrigger("attack3");
                break;
        }

        // 공격 애니메이션이 끝날 때까지 대기
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float attackAnimationDuration = stateInfo.length;
        yield return new WaitForSeconds(attackAnimationDuration);

        isAttacking = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            UpdateState(CurrentState.Alert);

            if (mainCollider != null)
            {
                mainCollider.enabled = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            if (!hasWokenUp)
            {
                UpdateState(CurrentState.Sleep);
            }
            else
            {
                UpdateState(CurrentState.Alert);
            }
        }
    }
}