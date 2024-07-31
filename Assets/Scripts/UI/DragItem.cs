using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : Drag
{
    private StateUiSlot parentSlot = null;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        parentSlot = myParent.GetComponent<StateUiSlot>();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (parentSlot != null)
        {
            parentSlot.CheckAndTakeOffIfNeeded(gameObject);
        }
    }
}