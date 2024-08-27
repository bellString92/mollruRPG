using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIPerception : MonoBehaviour
{
    public float detectionRadius = 9.0f;
    public LayerMask enemyMask;
    public UnityEvent<Transform> findAct;
    public UnityEvent lostAct;
    protected Transform myTarget = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & enemyMask) != 0)
        {
            if (myTarget == null)
            {
                myTarget = other.transform;
                findAct?.Invoke(other.transform);
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
        StartCoroutine(CheckForEnemies());
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
            
            yield return new WaitForSeconds(0.5f); // 0.5초마다 실행

            if (enemies.Length < 0)
            {
                StopCoroutine(CheckForEnemies());
                break;
            }
        }
    }
}
