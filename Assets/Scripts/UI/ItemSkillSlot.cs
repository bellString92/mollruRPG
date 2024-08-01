using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSkillSlot : MonoBehaviour, IDropHandler, ISetChild, IPointerClickHandler
{
    public GameObject myChild = null;
    public SlotType slotType = SlotType.Item;
    public Transform dragItemSkill;
    
    public void OnDrop(PointerEventData eventData)
    {
        if (slotType.Equals(eventData.pointerDrag.GetComponent<Drag>().slotType) ||
            (slotType.Equals(SlotType.SlotItem) && eventData.pointerDrag.GetComponent<Drag>().slotType.Equals(SlotType.Item)) ||
            (slotType.Equals(SlotType.SlotSkill) && eventData.pointerDrag.GetComponent<Drag>().slotType.Equals(SlotType.Skill)))
        {
            Transform slotItem;
            if (eventData.pointerDrag.GetComponent<Drag>().slotType.Equals(SlotType.SlotItem) ||
                eventData.pointerDrag.GetComponent<Drag>().slotType.Equals(SlotType.SlotSkill))
            {
                slotItem = eventData.pointerDrag.transform;
            }
            else
            {
                slotItem = GameObject.Instantiate(eventData.pointerDrag.transform);
                slotItem.name = slotType.ToString();
                slotItem.GetComponent<Drag>().slotType = slotType.Equals(SlotType.Item) || slotType.Equals(SlotType.SlotItem) ? SlotType.SlotItem : SlotType.SlotSkill;
            }
            slotItem.GetComponent<Image>().raycastTarget = true;

            slotItem.SetParent(transform);
            
            RectTransform rt = slotItem as RectTransform;
            Vector2 anchor = new Vector2(0, 0);
            rt.anchorMin = anchor;
            anchor.x = 1; anchor.y = 1;
            rt.anchorMax = anchor;
            anchor.x = 0.5f; anchor.y = 0.5f;
            rt.pivot = anchor;
            anchor.x = 0; anchor.y = 0;
            rt.offsetMin = anchor;
            rt.offsetMax = anchor;

            if (eventData.pointerDrag.GetComponent<Drag>().slotType.Equals(SlotType.SlotItem) || eventData.pointerDrag.GetComponent<Drag>().slotType.Equals(SlotType.SlotSkill))
            {
                if (myChild != null)
                {
                    myChild.GetComponent<ISwapParent>()?.SwapParent(eventData.pointerDrag.GetComponent<IGetParent>().myParent);
                }
                else
                {
                    eventData.pointerDrag.GetComponent<IGetParent>()?.myParent.GetComponent<ISetChild>().SetChild(null);
                }
                eventData.pointerDrag.GetComponent<IChangeParent>()?.ChangeParent(transform);
            }
            else
            {
                Destroy(myChild);
                foreach (Transform obj in transform.parent)
                {
                    if (obj == transform) continue;
                    if (obj.GetComponent<ItemSkillSlot>().slotType.Equals(SlotType.SlotSkill)) {
                        if (obj.GetComponentInChildren<PlayerSkill>() == null) continue;
                        if (obj.GetComponentInChildren<PlayerSkill>().skill.Equals(eventData.pointerDrag.GetComponent<PlayerSkill>().skill))
                        {
                            Destroy(obj.GetComponentInChildren<PlayerSkill>().gameObject);
                        }
                    }
                }
            }

            myChild = slotItem.gameObject;
            myChild.GetComponent<Drag>().destroyChk = false;
            transform.parent.GetComponent<SkillController>().myImg[transform.GetSiblingIndex()] = myChild.GetComponentsInChildren<Image>()[1];
            transform.parent.GetComponent<SkillController>().myLabel[transform.GetSiblingIndex()] = myChild.GetComponentsInChildren<TMPro.TMP_Text>()[0];
        }
    }

    public void SetChild(GameObject newChild)
    {
        myChild = newChild;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
