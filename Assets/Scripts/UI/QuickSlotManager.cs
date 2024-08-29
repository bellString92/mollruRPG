using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotManager : MonoBehaviour
{
    public static QuickSlotManager Instance;
    public List<ItemSkillSlot> slots = new List<ItemSkillSlot>();

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1) && slots.Count >= 1)
        {
            if (slots[0].myChild != null)
            { 
                slots[0].UseItem();
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad2) && slots.Count >= 2)
        {
            if (slots[1].myChild != null)
            {
                slots[1].UseItem();
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad3) && slots.Count >= 3)
        {
            if (slots[2].myChild != null)
            {
                slots[2].UseItem();
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad4) && slots.Count >= 4)
        {
            if (slots[3].myChild != null)
            {
                slots[3].UseItem();
            }
        }
    }
    public ItemSkillSlot FindSlotByItemID(int itemID)
    {
        foreach (ItemSkillSlot slot in slots)
        {
            if (slot.myChild != null)
            {
                SaveItemInfo saveItemInfo = slot.myChild.GetComponent<SaveItemInfo>();
                if (saveItemInfo != null && saveItemInfo.item.itemID == itemID)
                {
                    return slot; // 해당 아이템이 이미 등록된 슬롯 반환
                }
            }
        }
        return null; // 해당 아이템이 등록되지 않음
    }
}
