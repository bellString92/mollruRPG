using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSkillSlot : MonoBehaviour, IDropHandler, ISetChild, IPointerClickHandler
{
    public GameObject myChild = null;
    public SlotType slotType = SlotType.UseItem;
    public Transform dragItemSkill;
    
    public void OnDrop(PointerEventData eventData)
    {
        if (slotType.Equals(eventData.pointerDrag.GetComponent<Drag>().slotType) ||
            (slotType.Equals(SlotType.SlotItem) && eventData.pointerDrag.GetComponent<Drag>().slotType.Equals(SlotType.UseItem)) ||
            (slotType.Equals(SlotType.SlotSkill) && eventData.pointerDrag.GetComponent<Drag>().slotType.Equals(SlotType.Skill)))
        {
            //if (SkillController.Instance.coCool[transform.GetSiblingIndex()] != null) return;
            Transform slotItem;
            Transform comboSkill = null;
            if (eventData.pointerDrag.GetComponent<Drag>().slotType.Equals(SlotType.SlotItem) ||
                eventData.pointerDrag.GetComponent<Drag>().slotType.Equals(SlotType.SlotSkill))
            {
                slotItem = eventData.pointerDrag.transform;
                if (slotItem.GetComponent<PlayerSkill>() != null)
                    comboSkill = slotItem.GetComponent<PlayerSkill>().comboSkill;
            }
            else
            {
                var draggedItemInfo = eventData.pointerDrag?.GetComponent<SaveItemInfo>();
                // 드래그된 아이템을 새로 생성하여 슬롯에 설정
                slotItem = GameObject.Instantiate(eventData.pointerDrag.transform);
                slotItem.name = eventData.pointerDrag.transform.name;
                slotItem.GetComponent<Drag>().slotType = slotType.Equals(SlotType.UseItem) || slotType.Equals(SlotType.SlotItem) ? SlotType.SlotItem : SlotType.SlotSkill;

                // 아이템 타입이 consumItem인 경우 추가 로직 실행
                if (draggedItemInfo != null && draggedItemInfo.item.itemType == ItemType.consumItem)
                {
                    if (slotItem.GetComponent<PlayerSkill>() != null && slotItem.GetComponent<PlayerSkill>().comboSkill != null)
                    {
                        comboSkill = GameObject.Instantiate(slotItem.GetComponent<PlayerSkill>().comboSkill);
                    }
                }
                else
                {
                    // 아이템 타입이 consumItem이 아닌 경우에 대한 처리
                    Debug.LogWarning("올바르지 않은 아이템 타입입니다.");
                    return;
                }
            }

            if (comboSkill != null)
            {
                comboSkill.name = slotItem.GetComponent<PlayerSkill>().comboSkill.name;
                slotItem.GetComponent<PlayerSkill>().comboSkill = comboSkill;
                comboSkill.gameObject.SetActive(false);
                comboSkill.SetParent(slotItem);
                RectTransform srt = comboSkill as RectTransform;
                Vector2 sanchor = new Vector2(0, 0);
                srt.anchorMin = sanchor;
                sanchor.x = 1; sanchor.y = 1;
                srt.anchorMax = sanchor;
                sanchor.x = 0.5f; sanchor.y = 0.5f;
                srt.pivot = sanchor;
                sanchor.x = 0; sanchor.y = 0;
                srt.offsetMin = sanchor;
                srt.offsetMax = sanchor;
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
                //foreach (Transform obj in transform.parent)
                int i = 0;
                int e = transform.parent.childCount;
                if (eventData.pointerDrag.GetComponent<Drag>().slotType.Equals(SlotType.SlotItem)
                    || eventData.pointerDrag.GetComponent<Drag>().slotType.Equals(SlotType.UseItem)) e = 4;
                if (eventData.pointerDrag.GetComponent<Drag>().slotType.Equals(SlotType.SlotSkill)
                    || eventData.pointerDrag.GetComponent<Drag>().slotType.Equals(SlotType.Skill)) i = 4;

                for (; i < e; i++)
                {
                    if (transform.parent.GetChild(i) == transform) continue;
                    if (transform.parent.GetChild(i).GetComponent<ItemSkillSlot>().slotType.Equals(SlotType.SlotSkill)) {
                        if (transform.parent.GetChild(i).GetComponentInChildren<PlayerSkill>() == null) continue;
                        if (transform.parent.GetChild(i).GetComponentInChildren<PlayerSkill>().skill.Equals(eventData.pointerDrag.GetComponent<PlayerSkill>().skill))
                        {
                            Destroy(transform.parent.GetChild(i).GetComponentInChildren<PlayerSkill>().gameObject);
                        }
                    }
                }
            }

            myChild = slotItem.gameObject;
            myChild.GetComponent<Drag>().destroyChk = false;
            if (slotType.Equals(SlotType.Skill) || slotType.Equals(SlotType.SlotSkill))
            {
                transform.parent.GetComponent<SkillController>().myImg[transform.GetSiblingIndex()] = myChild.GetComponentsInChildren<Image>()[1];
                transform.parent.GetComponent<SkillController>().myLabel[transform.GetSiblingIndex()] = myChild.GetComponentsInChildren<TMPro.TMP_Text>()[0];
            }

        }
        else if ((slotType.Equals(SlotType.SlotItem) && eventData.pointerDrag.GetComponent<Drag>().slotType.Equals(SlotType.SlotSkill))
            || (slotType.Equals(SlotType.SlotSkill) && eventData.pointerDrag.GetComponent<Drag>().slotType.Equals(SlotType.SlotItem)))
        {

            eventData.pointerDrag.transform.gameObject.GetComponent<Drag>().destroyChk = false;
        }
    }

    public void SetChild(GameObject newChild)
    {
        myChild = newChild;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            UseItem();
        }
    }

    public void UseItem()
    {
        myChild.GetComponent<SaveItemInfo>()?.UseItem(UIManager.Instance.player.GetComponent<Player>());
    }
}
