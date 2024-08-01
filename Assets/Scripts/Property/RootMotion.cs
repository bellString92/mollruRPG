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
        // 레이 생성 deltaPosition.normalized
        Ray ray = new Ray(transform.parent.position + (Vector3.up * 0.1f), Vector3.down);

        // 레이 캐스트
        Physics.SphereCast(ray, radius, out RaycastHit hit, 2.0f, crashMask);

        // 각도 구함
        var angle = Vector3.Angle(Vector3.up, hit.normal);

        // 실제 이동
        if (!Physics.SphereCast(ray, radius, out RaycastHit hitzz, 2.0f, crashMask))
        {
            if (angle < 0.1f) { rb.velocity = deltaPosition; }
        }
        Debug.DrawRay(transform.parent.position + (Vector3.up * 0.1f), Vector3.down * 2.0f, UnityEngine.Color.red, 1.0f);

        // 초기화
        transform.parent.rotation *= deltaRotation;
        deltaPosition = Vector3.zero;
        deltaRotation = Quaternion.identity;
    }

    private void OnAnimatorMove()
    {
        // 이동속도 버프용
        Vector3 rootMotion = myAnim.deltaPosition;                                                      // 루트 모션에서 추출된 deltaPosition을 가져옵니다.
        Vector3 scaledMotion = rootMotion * transform.parent.GetComponent<Player>().myStat.moveSpeed;   // 이동 속도 배수를 적용하여 새로운 deltaPosition을 계산합니다.
        Vector3 move = new Vector3(scaledMotion.x, -1.5f, scaledMotion.z);

        deltaPosition += move;
        deltaRotation *= myAnim.deltaRotation;
    }
}
