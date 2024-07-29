using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RootMotion : AnimatorProperty
{
    public LayerMask crashMask;

    Vector3 deltaPosition = Vector3.zero;
    Vector3 DonwPosition = new Vector3(0, -1, 0);
    Quaternion deltaRotation = Quaternion.identity;

    float radius = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        CapsuleCollider col = transform.parent.GetComponent<CapsuleCollider>();
        if(col != null)
        {
            radius = col.radius;
        }
    }

    // Update is called once per frame
    void Update()
    { }

    private void FixedUpdate()
    {
        Ray ray = new Ray(transform.parent.position + (Vector3.up * 0.1f), deltaPosition.normalized);           // 레이 생성
        Physics.Raycast(ray, out RaycastHit hit, deltaPosition.magnitude, crashMask);
        var angle = Vector3.Angle(Vector3.up, hit.normal);

        if (angle > 45.0f)                       // 레이 캐스트를 쏨
        {

                //Debug.Log("45보다 큼");
                Debug.DrawRay(ray.origin, ray.direction * hit.distance * 100, Color.green); // 충돌한 경우
                transform.parent.position += ray.direction * (hit.distance - radius);   
        }
        else
        {
                //Debug.Log("45보다 작음");
                Debug.DrawRay(ray.origin, ray.direction * deltaPosition.magnitude * 100, Color.red); // 충돌하지 않은 경우
                transform.parent.position += deltaPosition;
        }

        transform.parent.rotation *= deltaRotation;
        deltaPosition = Vector3.zero;
        deltaRotation = Quaternion.identity;
    }

    private void OnAnimatorMove()
    {
        // 이동속도 버프용
        Vector3 rootMotion = myAnim.deltaPosition;                                                      // 루트 모션에서 추출된 deltaPosition을 가져옵니다.
        Vector3 scaledMotion = rootMotion * transform.parent.GetComponent<Player>().myStat.moveSpeed;   // 이동 속도 배수를 적용하여 새로운 deltaPosition을 계산합니다.

        deltaPosition += scaledMotion; //myAnim.deltaPosition;
        deltaRotation *= myAnim.deltaRotation;
    }
    
}
