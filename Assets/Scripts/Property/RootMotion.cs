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
    bool Check = true;
    bool RotCheck = false;

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
        // 실제이동
        rb.velocity = deltaPosition;
        transform.parent.rotation *= deltaRotation;

        // 초기화
        deltaPosition = Vector3.zero;
        deltaRotation = Quaternion.identity;
    }

    private void OnAnimatorMove()
    {
        // 이동 관련
        Vector3 scaledMotion = myAnim.deltaPosition * transform.parent.GetComponent<Player>().myStat.moveSpeed;   // 이동 속도 배수를 적용하여 새로운 deltaPosition을 계산합니다.
        Quaternion scaledRotation = Quaternion.Euler(0, myAnim.GetBool("Right") ? 45 : myAnim.GetBool("Left") ? -45 : 0, 0);
        Vector3 move = new Vector3(scaledMotion.x, 0, scaledMotion.z); //scaledRotation * scaledMotion;

        //회전 관련

        if ((myAnim.GetBool("Right") || myAnim.GetBool("Left")))
        {
            transform.rotation = transform.parent.rotation * scaledRotation;
        }
        else { transform.rotation = transform.parent.rotation; }


        // 레이캐스트를 발사하여 바닥에 닿아 있는지 확인
        isGrounded = Physics.Raycast(transform.parent.position + (Vector3.up * 0.03f), Vector3.down, groundCheckDistance, crashMask);
        Debug.DrawRay(transform.parent.position + (Vector3.up * 0.03f), Vector3.down * groundCheckDistance, UnityEngine.Color.red);

        // 땅이 있으면
        move.y = isGrounded ? -0.5f : -5.0f;

        deltaPosition += move;
        deltaRotation *= myAnim.deltaRotation;
    }
}
