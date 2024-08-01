using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

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
    public SlotType slotType = SlotType.UseItem;
    public bool destroyChk = false;

    public Transform myParent { get; private set; }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        
        myParent = transform.parent;
        if (transform.parent.GetComponent<ItemSkillSlot>() == null)
        {
            if (slotType.Equals(SlotType.UseItem))
                transform.SetParent(transform.parent.parent.parent.parent.parent.GetComponent<Inventory>().dragItem.transform);
            else if (slotType.Equals(SlotType.Skill))
                transform.SetParent(transform.parent.parent.GetComponent<SkillSlot>().dragSkill.transform);
        }
        else
        {
            transform.SetParent(transform.parent.GetComponent<ItemSkillSlot>().dragItemSkill.transform);
        }

        if (slotType.Equals(SlotType.SlotItem) || slotType.Equals(SlotType.SlotSkill))
            destroyChk = true;
        myImage.raycastTarget = false;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (destroyChk)
        {
            Destroy(transform.gameObject);
        }
        else
        {
            transform.SetParent(myParent);
            transform.localPosition = Vector2.zero;
            myImage.raycastTarget = true;
        }
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
