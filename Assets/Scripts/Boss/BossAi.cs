using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
// 몬스터 회전 값 Movement에서 상속받아서 변경하기

public enum CurrentState { Sleep, Alert, Chase, Attack, Enraged, Dead }

public class BossAI : MonoBehaviour
{
    // 공개 파라미터
    [Header("Game Settings")]
    public float moveSpeed;
    public float attackRange;
    public float wakeUpRange;
    public float attackStateRange;
    public int maxHP;
    public CurrentState currentState;
    public Transform player;
    //public Animator animator;


    // 비공개 파라미터
    private int currentHealth;
    private Animator animator;
    //private Transform player;
    private Collider sphereCollider;
    //private CurrentState currentState;
    private bool hasWokenUp = false; // 한 번 깨어났는지를 추적
    private float timer;
    private float delaytimer;
    NavMeshAgent agent;


    void Start()
    {
        currentHealth = maxHP;
        animator = GetComponent<Animator>();
        sphereCollider = GetComponent<Collider>();
        currentState = CurrentState.Sleep;
        UpdateState(CurrentState.Sleep);

        timer = 0.0f;
        delaytimer = 2.0f;


    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

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
                MoveTowardsPlayer();
                if (distanceToPlayer <= attackRange)
                {
                    UpdateState(CurrentState.Attack);
                }
                /*else if (distanceToPlayer >= wakeUpRange)
                {
                    UpdateState(CurrentState.Alert); // Sleep 상태로 돌아가지 않도록 Alert 상태로 전환
                }*/
                break;

            case CurrentState.Attack:
                if (distanceToPlayer < attackRange)
                {
                    Attack();
                }
                else
                {
                    UpdateState(CurrentState.Chase);
                }
                break;

            case CurrentState.Enraged:
                if (distanceToPlayer < attackRange)
                {
                    Attack();
                }
                else
                {
                    MoveTowardsPlayer();
                }
                break;

            case CurrentState.Dead:// 죽음 처리
                break;
        }
    }

    public void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        animator.SetBool("isMoving", true);
    }

    public void Attack() //코루틴으로 변경해야함
    {
        
        int attackIndex = Random.Range(0, 3); // 0, 1, 2 중 무작위 선택
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
        animator.SetBool("isMoving", false);
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
                break;
            case CurrentState.Alert:
                animator.SetBool("isSleeping", false);
                animator.SetBool("isFind", true);
                hasWokenUp = true; // 보스가 한 번 깨어났음을 표시
                break;
            case CurrentState.Chase:
                animator.SetBool("isMoving", true);
                animator.SetBool("isSleeping", false);
                break;
            case CurrentState.Attack:
                Attack();
                //animator.SetBool("Attack", true);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            UpdateState(CurrentState.Alert);

            // 스피어 콜라이더 비활성화
            if (sphereCollider != null)
            {
                sphereCollider.enabled = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            if (!hasWokenUp) // 깨어난 적이 없다면 수면 상태로 전환
            {
                UpdateState(CurrentState.Sleep);
            }
            else // 한 번 깨어났다면 Idle 상태로 유지
            {
                UpdateState(CurrentState.Alert);
            }
        }
    }
}