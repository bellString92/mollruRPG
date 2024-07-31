using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStateUiManager : MonoBehaviour
{
    public static PlayerStateUiManager Instance;

    public Transform weaponSlot;
    public Transform armorSlot;
    public Transform acceSlot;
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
                slotTransform = armorSlot;
                break;
            case ItemType.acceItem:
                slotTransform = acceSlot;
                break;
        }

        if (slotTransform != null)
        {
            StateUiSlot slot = slotTransform.GetComponent<StateUiSlot>();
            if (slot.myChild != null)
            {
                GameObject existingItem = slot.myChild;
                existingItem.GetComponent<SaveItemInfo>()?.itemKind.TakeOff(user);// 기존 아이템 능력치 제거
                Inventory.Instance.AddItem(existingItem); // 인벤토리의 빈 슬롯을 찾아 이동
            }
            item.transform.SetParent(slotTransform);
            item.transform.localPosition = Vector2.zero;
            slot.SetChild(item);
            slot.curChild = item;
            item.GetComponent<SaveItemInfo>()?.itemKind.Use(user); // 새 아이템 능력치 적용

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
