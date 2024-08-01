using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStateUiManager : MonoBehaviour
{
    public static PlayerStateUiManager Instance;

    public Transform weaponSlot;
    public Transform headArmorSlot;
    public Transform chestArmorSlot;
    public Transform glovesArmorSlot;
    public Transform bootsArmorSlot;
    public Transform NecklaceSlot;
    public Transform RingSlot;
    public Transform SecondRingSlot;
    StateUiSlot slot;

    public Player user;
    public TextMeshProUGUI showMyState;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Update()
    {
        UpdatePlayerStats();
    }

    public void SetSlot(GameObject item, ItemType itemType)
    {
        Transform slotTransform = null;

        switch (itemType)
        {
            case ItemType.weaponItem:
                slotTransform = weaponSlot;
                break;
            case ItemType.armorItem:
                var armorItem = item.GetComponent<SaveItemInfo>()?.item as ArmorItem;
                if (armorItem != null)
                {
                    switch (armorItem.armorType)
                    {
                        case ArmorType.Head:
                            slotTransform = headArmorSlot;
                            break;
                        case ArmorType.Chest:
                            slotTransform = chestArmorSlot;
                            break;
                        case ArmorType.Gloves:
                            slotTransform = glovesArmorSlot;
                            break;
                        case ArmorType.Boots:
                            slotTransform = bootsArmorSlot;
                            break;
                        default:
                            break;
                    }
                }
                break;
            case ItemType.acceItem:
                var acceItem = item.GetComponent<SaveItemInfo>()?.item as AcceItem;
                if (acceItem != null)
                {
                    switch (acceItem.AcceType)
                    {
                        case AcceType.Necklace:
                            slotTransform = NecklaceSlot;
                            break;
                        case AcceType.Ring:
                            // 반지 슬롯 중 빈 슬롯을 찾음
                            if (RingSlot.GetComponent<StateUiSlot>()?.myChild == null)
                            {
                                slotTransform = RingSlot;
                            }
                            else if (SecondRingSlot.GetComponent<StateUiSlot>()?.myChild == null)
                            {
                                slotTransform = SecondRingSlot;
                            }
                            else
                            {
                                // 두 슬롯 모두 차있을 경우 첫 번째 슬롯으로 설정
                                slotTransform = RingSlot;
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
        }

        if (slotTransform != null)
        {
            StateUiSlot slot = slotTransform.GetComponent<StateUiSlot>();
            if (slot.myChild != null)
            {
                GameObject existingItem = slot.myChild;
                existingItem.GetComponent<SaveItemInfo>()?.item.TakeOff(user);// 기존 아이템 능력치 제거
                Inventory.Instance.AddItem(existingItem); // 인벤토리의 빈 슬롯을 찾아 이동
            }
            item.transform.SetParent(slotTransform);
            item.transform.localPosition = Vector2.zero;
            slot.SetChild(item);
            slot.curChild = item;
            item.GetComponent<SaveItemInfo>()?.item.Use(user); // 새 아이템 능력치 적용

            // 능력치 업데이트
            //UpdatePlayerStats();
        }
    }

    public void UpdatePlayerStats()
    {
        if (user != null && showMyState != null)
        {
            BattleStat stats = user.myStat;
            string playerStats = $"[닉네임]\n" +
                                 $"  레   벨   : {stats.myLvevel}\n" +
                                 $" 경 험 치  : {stats.curExperiencePoint} / {stats.maxExperiencePoint}\n" +
                                 $"  체   력   : {stats.curHealPoint} / {stats.maxHealPoint}\n" +
                                 $"  마   나   : {stats.curMagicPoint} / {stats.maxMagicPoint}\n" +
                                 $" 공 격 력  : {stats.AttackPoint}\n" +
                                 $"치명 확률 : {stats.CriticalProbability:F2}%\n" +
                                 $"치명 피해 : {stats.CriticalDamage * 100:F2}%";

            showMyState.text = playerStats;
        }
    }

}
