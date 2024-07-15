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
        // Scrollbar�� ���� TextMeshPro�� ScrollRect verticalNormalizedPosition�� ���ε�
        scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);
    }

    // Scrollbar �� ���� �� ȣ��Ǵ� �Լ�
    void OnScrollbarValueChanged(float value)
    {
        textMeshPro.rectTransform.anchoredPosition = new Vector2(
            textMeshPro.rectTransform.anchoredPosition.x,
            Mathf.Lerp(0, textMeshPro.preferredHeight - textMeshPro.rectTransform.rect.height, value)
        );
    }
}
