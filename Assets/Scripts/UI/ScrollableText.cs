using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollableText : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public Scrollbar scrollbar;

    void Start()
    {
        // Scrollbar의 값을 TextMeshPro의 ScrollRect verticalNormalizedPosition에 바인딩
        scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);
    }

    // Scrollbar 값 변경 시 호출되는 함수
    void OnScrollbarValueChanged(float value)
    {
        textMeshPro.rectTransform.anchoredPosition = new Vector2(
            textMeshPro.rectTransform.anchoredPosition.x,
            Mathf.Lerp(0, textMeshPro.preferredHeight - textMeshPro.rectTransform.rect.height, value)
        );
    }
}
