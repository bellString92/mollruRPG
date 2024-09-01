using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.Events;

public class AIPerception : MonoBehaviour
{
    public float detectionRadius = 9.0f;
    public LayerMask enemyMask;
    public UnityEvent<Transform> findAct;
    public UnityEvent lostAct;
    protected Transform myTarget = null;
    private Coroutine detectionCoroutine;
    // Start is called before the first frame update
    /*void Start()
    {
        
    }

    // Update is called once per frame
    //void Update()
    //{
        //StartCoroutine(CheckForEnemies());
    //}

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & enemyMask) != 0)
        {
            if (myTarget == null)
            {
                myTarget = other.transform;
                findAct?.Invoke(myTarget);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (myTarget == other.transform)
        {
            myTarget = null;
            lostAct?.Invoke();
        }
    }

    public void SetEnable(bool v)
    {
        gameObject.SetActive(v);
    }

    void OnEnable()
    {
        if (detectionCoroutine != null)
        {
            StopCoroutine(detectionCoroutine);
        }
        detectionCoroutine = StartCoroutine(CheckForEnemies());
    }

    void OnDisable()
    {
        if (detectionCoroutine != null)
        {
            StopCoroutine(detectionCoroutine);
            detectionCoroutine = null;
        }
    }

    IEnumerator CheckForEnemies()
    {
        while (true)
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyMask);
            if (enemies.Length > 0)
            {
                myTarget = enemies[0].transform;
                findAct?.Invoke(myTarget);
            }
            else
            {
                if (myTarget != null)
                {
                    myTarget = null;
                    lostAct?.Invoke();
                }
            }

            yield return new WaitForSeconds(0.5f); // 0.5초마다 실행
        }
    }*/
    void OnEnable()
    {
        // 리스폰 후 타겟 감지 시작
        StartCoroutine(DetectTargetRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        // 주기적으로 타겟 감지
        DetectTarget();
    }

    private IEnumerator DetectTargetRoutine()
    {
        // 리스폰 후 짧은 딜레이를 추가할 수 있음 (예: 0.5초)
        yield return new WaitForSeconds(0.5f);
        DetectTarget();
    }

    private void DetectTarget()
    {
        // 현재 위치에서 지정된 반경 내의 적을 모두 탐지
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyMask);

        if (enemies.Length > 0)
        {
            // 첫 번째 적을 타겟으로 설정
            Transform newTarget = enemies[0].transform;

            if (myTarget == null || myTarget != newTarget)
            {
                myTarget = newTarget;
                findAct?.Invoke(myTarget);
            }
        }
        else
        {
            // 적이 없으면 타겟을 null로 설정
            if (myTarget != null)
            {
                myTarget = null;
                lostAct?.Invoke();
            }
        }
    }

    public void SetEnable(bool v)
    {
        gameObject.SetActive(v);
    }

}
