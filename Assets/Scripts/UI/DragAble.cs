using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAble : ImageProperty, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected Vector2 dragOffset = Vector2.zero;
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        dragOffset = (Vector2)transform.position - eventData.position;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position + dragOffset;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {

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
