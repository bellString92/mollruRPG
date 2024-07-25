using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


    public enum CurrentState { Sleep, Alert, Chase, Attack, Enraged, Dead }

    public class ex : MonoBehaviour
    {
        [Header("Game Settings")]
        public float moveSpeed;
        public float attackRange;
        public float wakeUpRange;
        public float attackStateRange;
        public int maxHP;
        public CurrentState currentState;
        public Transform player;

        private int currentHealth;
        private Animator animator;
        private Collider mainCollider;
        private SphereCollider sleepCollider;
        private GameObject sleepColliderObject;
        private bool hasWokenUp = false;
        private float timer;
        private float delaytimer;
        private NavMeshAgent agent;
        private bool isAttacking = false;

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
                Debug.LogError("SleepCollider not found. Please ensure there is a child object named 'SleepCollider' with a SphereCollider attached.");
                return;
            }

            currentHealth = maxHP;

            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component not found. Please ensure there is an Animator component on the Avatar object.");
                return;
            }

            mainCollider = GetComponent<Collider>();
            agent = GetComponent<NavMeshAgent>();

            currentState = CurrentState.Sleep;
            UpdateState(CurrentState.Sleep);

            timer = 0.0f;
            delaytimer = 2.0f;
        }

        void Update()
        {
            if (player == null) return;

            float distanceToPlayer = Vector3.Distance(player.position, transform.position);

            // 항상 플레이어를 바라보게
            if (!isAttacking) // 공격중에는 플레이어를 바라보지않게
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

            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // 바라보는 속도
        }

        private void MoveIfInChaseAnimation()
        {
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
                    StartCoroutine(Attack());
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

