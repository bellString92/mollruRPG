using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeEffectCube: MonoBehaviour
{
    public float fixedScaleX = 6f;       // 고정된 X 스케일 값
    public float fixedScaleY = 0.1f;     // 고정된 Y 스케일 값
    public float initialScaleZ = 2f;     // 초기 Z 스케일
    public float finalScaleZ = 10f;      // 최종 Z 스케일
    public float duration = 1f;          // 스케일이 변화하는 시간
    public float fadeDuration = 1f;      // 사라지는 시간

    private float currentTime = 0f;      // 현재 경과 시간
    private Material material;           // 프리팹의 Material을 저장할 변수

    void Start()
    {
        // 프리팹의 초기 스케일 설정
        transform.localScale = new Vector3(fixedScaleX, fixedScaleY, initialScaleZ);

        // 프리팹의 Material 가져오기
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
        }
    }

    void Update()
    {
        // 스케일 변경
        if (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / duration; // 0에서 1까지의 비율

            // Z 스케일 변화
            float scaleZ = Mathf.Lerp(initialScaleZ, finalScaleZ, t);

            // X 스케일과 Y 스케일은 고정하고, Z 스케일만 변경
            transform.localScale = new Vector3(fixedScaleX, fixedScaleY, scaleZ);

            // 한쪽 면이 고정된 상태로 다른 쪽으로 커지게 하기 위해 위치를 조정
            // 예를 들어 Z 방향으로 커질 때 Z 위치를 조정할 수 있습니다.
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
        else
        {
            // 스케일 변화가 완료되면 서서히 사라지기 시작
            float fadeT = (currentTime - duration) / fadeDuration;
            Color color = material.color;
            color.a = Mathf.Lerp(1f, 0f, fadeT); // 알파 값 변경
            material.color = color;

            // 알파 값이 0이 되면 오브젝트 제거
            if (color.a <= 0f)
            {
                Destroy(gameObject);
            }
        }
        currentTime += Time.deltaTime;
    }
}
