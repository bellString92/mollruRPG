using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class RootMotion : AnimatorProperty
{
    public LayerMask crashMask;                         // 충돌 체크

    Vector3 deltaPosition = Vector3.zero;               // 나의 이동
    Quaternion deltaRotation = Quaternion.identity;     // 나의 방향

    float radius = 0.0f;                                // 나의 몸의 크기
    Rigidbody rb;                                       // 나의 리지드바디

    // Start is called before the first frame update
    void Start()
    {
        CapsuleCollider col = transform.parent.GetComponent<CapsuleCollider>();
        if (col != null)
        {
            radius = col.radius;
        }

        Rigidbody rg = transform.parent.GetComponent<Rigidbody>();
        if (rg != null)
        {
            rb = rg.GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        // 이동
        rb.velocity = deltaPosition + (Vector3.up * rb.velocity.y) * transform.parent.GetComponent<Player>().myStat.moveSpeed;
        transform.parent.rotation *= deltaRotation;

        Debug.Log(rb.velocity.y);
        
        // 초기화
        deltaPosition = Vector3.zero;
        deltaRotation = Quaternion.identity;
    }

    private void OnAnimatorMove()
    {
        Vector3 move = new Vector3(deltaPosition.x, 0, deltaPosition.z);

        deltaPosition += move;
        deltaRotation *= myAnim.deltaRotation;
    }
}
