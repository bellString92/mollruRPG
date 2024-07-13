using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BezierCurve : MonoBehaviour
{
    public GameObject player;
    public GameObject target;
    public float activationDistance = 3.0f;
    private bool isJumping = false;
    private bool hasJumped = false;
    private float progress = 0.0f;

    public Vector3 P1;
    public Vector3 P2;
    public Vector3 P3;
    public Vector3 P4;

    public float jumpStart = 0.3f;
    public float jumpEnd = 0.7f;
    public float jumpHeight = 2.0f;

    private Animator animator;

    /*private void Start()
    {
        animator = player.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on player object.");
        }
    }*/

    private void Update()
    {
        float distanceToTarget = Vector3.Distance(player.transform.position, target.transform.position);

        if (distanceToTarget <= activationDistance && Input.GetKeyDown(KeyCode.F) && !isJumping)
        {
            isJumping = true;
            player.transform.SetParent(transform);
            animator.SetTrigger("Jump");// 점프 애니메이션 시작
        }

        if (isJumping)
        {
            if (progress < 1.0f)
            {
                if (progress < jumpStart || progress > jumpEnd)
                {
                    player.transform.position = BezierTest(P1, P2, P3, P4, progress);
                }
                else
                {
                    float jumpProgress = (progress - jumpStart) / (jumpEnd - jumpStart);
                    Vector3 groundPosition = BezierTest(P1, P2, P3, P4, progress);
                    player.transform.position = groundPosition + Vector3.up * Mathf.Sin(Mathf.PI * jumpProgress) * jumpHeight;
                }
                progress += Time.deltaTime; //점프 속도 조정
            }
            else
            {
                player.transform.SetParent(null);
                isJumping = false;
                hasJumped = true;
                progress = 0.0f;
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
