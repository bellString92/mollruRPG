using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeEffect : MonoBehaviour
{
    public float initialScale = 4f; // 초기 스케일 (X, Z)
    public float finalScale = 9f; // 최종 스케일 (X, Z)
    public float duration = 1f; // 스케일이 변화하는 시간
    public float fadeDuration = 1f; // 사라지는 시간
    public float fixedYScale = 0.1f; // 고정된 Y 스케일 값

    private float currentTime = 0f; // 현재 경과 시간
    private Material material; // 프리팹의 Material을 저장할 변수

    void Start()
    {
        // 프리팹의 초기 스케일 설정
        transform.localScale = new Vector3(initialScale, fixedYScale, initialScale);

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
            float scaleXZ = Mathf.Lerp(initialScale, finalScale, t); // X, Z 스케일 변화
            transform.localScale = new Vector3(scaleXZ, fixedYScale, scaleXZ); // X, Z 스케일은 변화하고 Y 스케일은 고정
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
