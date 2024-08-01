using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI textComponent;
    public Image backgroundImage;

    private RectTransform rectTransform;
    public float maxWidth = 500f; // 툴팁의 최대 너비
    public float padding = 20f;   // 패딩

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string text)
    {
        if (textComponent != null)
        {
            textComponent.text = text;
            AdjustSize();
        }
    }

    private void AdjustSize()
    {
        if (textComponent != null)
        {
            // 텍스트를 강제로 업데이트
            textComponent.ForceMeshUpdate();

            // 텍스트의 선호 사이즈를 계산
            var textSize = textComponent.GetPreferredValues(textComponent.text, maxWidth, 0);

            // 너비가 최대 너비를 초과할 경우 줄바꿈
            if (textSize.x > maxWidth)
            {
                textComponent.enableWordWrapping = true;
                textComponent.overflowMode = TextOverflowModes.Ellipsis;
                textSize.x = maxWidth;
            }

            // 텍스트 컨테이너의 사이즈를 조정
            rectTransform.sizeDelta = new Vector2(textSize.x + padding, textSize.y + padding);
        }
    }
}
