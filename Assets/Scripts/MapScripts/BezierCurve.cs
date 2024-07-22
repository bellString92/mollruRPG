using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BezierCurve : AnimatorProperty
{
    public GameObject player; //오브젝트 참조
    public DoorController linkedDoor;
    public GameObject avatar; // Player의 자식 오브젝트인 아바타 오브젝트 참조
    public GameObject target;
    public float activationDistance = 3.0f;
    private bool isJumping = false;
    private float progress = 0.0f;

    public Vector3 P1;
    public Vector3 P2;
    public Vector3 P3;
    public Vector3 P4;

    public float jumpStart = 0.3f;
    public float jumpEnd = 0.7f;
    public float jumpHeight = 2.0f;

    private Animator animator;

    private float interactionTime = 1.0f; // Required time to hold F to interact
    private float holdTime = 0.0f; // Time F has been held down

    private Transform savedTarget;

    private void Start()
    {
        if (avatar != null)
        {
            animator = avatar.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component not found on Avatar object.");
            }
        }
        else
        {
            Debug.LogError("Avatar object is not assigned.");
        }
    }

    private void Update()
    {
        if (animator == null)
        {
            Debug.LogError("Animator not assigned or found.");
            return;
        }

        /*if (!isJumping)
        {
            Transform currentPortal = FieldOfView.Instance.GetCurrentPortal();

            if (currentPortal != null)
            {
                target = currentPortal.gameObject;
            }
            else
            {
                target = null;
            }
        }*/

        if (target == null && savedTarget == null) return;

        float distanceToTarget = Vector3.Distance(player.transform.position, target.transform.position);

        // 현재 애니메이션 상태 정보를 가져옴
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

        if (distanceToTarget <= activationDistance && !isJumping && currentState.IsName("Move"))
        {
            if (Input.GetKey(KeyCode.F))
            {
               holdTime += Time.deltaTime;
                if (holdTime >= interactionTime)
                {
                    isJumping = true;
                    player.transform.SetParent(transform);
                    animator.SetTrigger("Jump"); // 점프 애니메이션 시작
                    savedTarget = target.transform; // 타겟 저장
                    holdTime = 0.0f;
                }
            }
            else
            {
                holdTime = 0.0f;
            }
        }

        if (isJumping)
        {
            if (progress < 1.0f)
            {
                Vector3 currentPos;
                if (progress < jumpStart || progress > jumpEnd)
                {
                    currentPos = BezierTest(P1, P2, P3, P4, progress);
                }
                else
                {
                    float jumpProgress = (progress - jumpStart) / (jumpEnd - jumpStart);
                    Vector3 groundPosition = BezierTest(P1, P2, P3, P4, progress);
                    currentPos = groundPosition + Vector3.up * Mathf.Sin(Mathf.PI * jumpProgress) * jumpHeight;
                }

                Vector3 nextPos = BezierTest(P1, P2, P3, P4, progress + 0.01f);
                Vector3 direction = (nextPos - currentPos).normalized;

                player.transform.position = currentPos;

                // 이동 방향을 유지하면서 기존의 회전 값 유지
                Quaternion currentRotation = player.transform.rotation;
                player.transform.rotation = Quaternion.LookRotation(direction);
                player.transform.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, player.transform.rotation.eulerAngles.y, currentRotation.eulerAngles.z);

                progress += Time.deltaTime; // 점프 속도 제어
            }
            else
            {
                player.transform.SetParent(null);
                isJumping = false;
                progress = 0.0f;
                animator.ResetTrigger("Jump");
                // gameObject.SetActive(false);
                savedTarget = null; // 점프 종료 후 타겟 해제
                Destroy(gameObject, 2);

                if (linkedDoor != null)
                {
                    linkedDoor.DungeonDoorClose();
                }
            }
        }
    }

    public Vector3 BezierTest(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4, float t)
    {
        Vector3 A = Vector3.Lerp(P1, P2, t);
        Vector3 B = Vector3.Lerp(P2, P3, t);
        Vector3 C = Vector3.Lerp(P3, P4, t);

        Vector3 D = Vector3.Lerp(A, B, t);
        Vector3 E = Vector3.Lerp(B, C, t);

        return Vector3.Lerp(D, E, t);
    }
}

[CanEditMultipleObjects]
[CustomEditor(typeof(BezierCurve))]
public class BezierCurveEditor : Editor
{
    private void OnSceneGUI()
    {
        BezierCurve curve = (BezierCurve)target;

        curve.P1 = Handles.PositionHandle(curve.P1, Quaternion.identity);
        curve.P2 = Handles.PositionHandle(curve.P2, Quaternion.identity);
        curve.P3 = Handles.PositionHandle(curve.P3, Quaternion.identity);
        curve.P4 = Handles.PositionHandle(curve.P4, Quaternion.identity);

        Handles.DrawLine(curve.P1, curve.P2);
        Handles.DrawLine(curve.P3, curve.P4);

        int count = 50;
        Vector3 prevPos = curve.P1;
        for (int i = 1; i <= count; i++)
        {
            float t = i / (float)count;
            Vector3 currentPos = curve.BezierTest(curve.P1, curve.P2, curve.P3, curve.P4, t);
            Handles.DrawLine(prevPos, currentPos);
            prevPos = currentPos;
        }
    }
}


