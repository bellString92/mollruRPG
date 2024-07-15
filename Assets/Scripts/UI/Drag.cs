using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

interface IChangeParent
{
    void ChangeParent(Transform parent);
}

interface ISwapParent
{
    void SwapParent(Transform newParent);
}

interface IGetParent
{
    Transform myParent { get; }
}


public class Drag : DragAble, IChangeParent, IChildObject, ISwapParent, IGetParent
{
    public Transform myParent { get; private set; }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        myParent = transform.parent;
        transform.SetParent(transform.parent.parent);
        myImage.raycastTarget = false;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(myParent);
        transform.localPosition = Vector2.zero;
        myImage.raycastTarget = true;
    }

    public void ChangeParent(Transform parent)
    {
        myParent = parent;
    }

    public void SwapParent(Transform newParent)
    {
        newParent.GetComponent<ISetChild>().SetChild(gameObject);
        transform.SetParent(newParent);
        transform.localPosition = Vector2.zero;
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
