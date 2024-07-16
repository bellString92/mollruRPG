using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public static FieldOfView Instance = null;

    // �þ� ������ �������� �þ� ����
    public float viewRadius; [Range(0, 360)]
    public float viewAngle;

    // ����ũ 2��
    public LayerMask targetMask, obstacleMask;

    // Target mask�� ray hit�� transform�� �����ϴ� ����Ʈ
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
        StartCoroutine(FindTargetsWithDelay(0.2f)); // 0.2�� �������� �ڷ�ƾ ȣ��
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
        visibleMonster.Clear(); //���빰 �ʱ�ȭ
        visibleNPC.Clear();
        visibleETC.Clear();


        // viewRadius�� ���������� �� �� ���� �� targetMask ���̾��� �ݶ��̴��� ��� ������
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // �÷��̾�� forward�� target�� �̷�� ���� ������ ���� �����
            dirToTarget.y = 0;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                // Ÿ������ ���� ����ĳ��Ʈ�� obstacleMask�� �ɸ��� ������ visibleMonster�� Add
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

    
    // ���� �þ� ���� ������Ż�� Ÿ���� �ִ��� Ȯ��
    public Transform GetCurrentTarget()
    {
        if (visibleMonster.Count > 0)
        {
            return visibleMonster[0];
        }
        return null;
    }

    // y�� ���Ϸ� ���� 3���� ���� ���ͷ� ��ȯ�Ѵ�.
    // ������ ������ ��¦ �ٸ��� ����. ����� ����.
    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }

    // ���� Ÿ�ٵ� ��Ż�� ��ȯ�ϴ� �޼��� �߰�
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
