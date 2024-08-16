using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DropItemCollectRange : AIPerception
{
    float closestDistance = Mathf.Infinity; // 가장 가까운 오브젝트까지의 거리

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & enemyMask) != 0)
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (myTarget == null || distance < closestDistance)
            {
                myTarget = other.transform;
                closestDistance = distance; // 업데이트된 가장 가까운 거리
                findAct?.Invoke(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (myTarget == other.transform)
        {
            myTarget = null;
            closestDistance = Mathf.Infinity; // 가장 가까운 거리 리셋
            lostAct?.Invoke();
        }
    }
}
