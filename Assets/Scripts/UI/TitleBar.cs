using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleBar : DragAble
{
    public override void OnBeginDrag(PointerEventData eventData)
    {
        dragOffset = (Vector2)transform.parent.position - eventData.position;
    }
    public override void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = eventData.position + dragOffset;
        float halfX = (transform.parent as RectTransform).rect.width * 0.5f;
        float halfY = (transform.parent as RectTransform).rect.height * 0.5f;
        pos.x = Mathf.Clamp(pos.x, halfX, Screen.width - halfX);
        pos.y = Mathf.Clamp(pos.y, halfY, Screen.height - halfY);
        transform.parent.position = pos;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
