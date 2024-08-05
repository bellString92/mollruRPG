using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class RootMotion : AnimatorProperty
{
    public LayerMask crashMask;

    Vector3 deltaPosition = Vector3.zero;
    Quaternion deltaRotation = Quaternion.identity;

    float radius = 0.0f;
    Rigidbody rb;
    bool isGrounded;
    float groundCheckDistance = 0.3f;

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
        rb.velocity = deltaPosition;
        transform.parent.rotation *= deltaRotation;

        // 초기화
        deltaPosition = Vector3.zero;
        deltaRotation = Quaternion.identity;
    }

    private void OnAnimatorMove()
    {
        // 이동속도 버프용
        Vector3 scaledMotion = myAnim.deltaPosition * transform.parent.GetComponent<Player>().myStat.moveSpeed;   // 이동 속도 배수를 적용하여 새로운 deltaPosition을 계산합니다.
        Vector3 move = new Vector3(scaledMotion.x, -1.0f, scaledMotion.z);

        // 레이캐스트를 발사하여 바닥에 닿아 있는지 확인
        isGrounded = Physics.Raycast(transform.parent.position + (Vector3.up * 0.03f), Vector3.down, groundCheckDistance, crashMask);
        Debug.DrawRay(transform.parent.position + (Vector3.up * 0.03f), Vector3.down * groundCheckDistance, UnityEngine.Color.red);
        Debug.Log(isGrounded);

        // 땅이 있으면
        move.y = isGrounded ? -0.5f : move.y = -10.0f;
        
        deltaPosition += move;
        deltaRotation *= myAnim.deltaRotation;
    }
}
