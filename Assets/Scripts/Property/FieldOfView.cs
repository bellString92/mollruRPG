using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public static FieldOfView Instance = null;

    // 시야 영역의 반지름과 시야 각도
    public float viewRadius; [Range(0, 360)]
    public float viewAngle;

    // 마스크 2종
    public LayerMask targetMask, obstacleMask;

    // Target mask에 ray hit된 transform을 보관하는 리스트
    public static List<Transform> visibleMonster = new List<Transform>();
    public static List<Transform> visibleNPC = new List<Transform>();
    public static List<Transform> visibleETC = new List<Transform>();

    public List<Transform> visibleMonsterView = visibleMonster;
    public List<Transform> visibleNPCView = visibleNPC;
    public List<Transform> visibleETCView = visibleETC;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(FindTargetsWithDelay(0.2f)); // 0.2초 간격으로 코루틴 호출
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindvisibleMonster();
        }
    }

    void FindvisibleMonster()
    {
        visibleMonster.Clear(); //내용물 초기화
        visibleNPC.Clear();
        visibleETC.Clear();


        // viewRadius를 반지름으로 한 원 영역 내 targetMask 레이어인 콜라이더를 모두 가져옴
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // 플레이어와 forward와 target이 이루는 각이 설정한 각도 내라면
            dirToTarget.y = 0;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                // 타겟으로 가는 레이캐스트에 obstacleMask가 걸리지 않으면 visibleMonster에 Add
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    if (target.CompareTag("Enemy"))
                    {
                        visibleMonster.Add(target);
                    }
                    else if (target.CompareTag("NPC"))
                    {
                        visibleNPC.Add(target);
                    }
                    else
                    {
                        visibleETC.Add(target);
                    }
                }
            }
        }
    }

    
    // 현재 시야 내에 점프포탈의 타겟이 있는지 확인
    public Transform GetCurrentTarget()
    {
        if (visibleMonster.Count > 0)
        {
            return visibleMonster[0];
        }
        return null;
    }

    // y축 오일러 각을 3차원 방향 벡터로 변환한다.
    // 원본과 구현이 살짝 다름에 주의. 결과는 같다.
    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }

    // 현재 타겟된 포탈을 반환하는 메서드 추가
    public Transform GetCurrentPortal()
    {
        foreach (Transform target in visibleMonster)
        {
            if (target.CompareTag("Portal"))
            {
                return target;
            }
        }
        return null;
    }
    
}
