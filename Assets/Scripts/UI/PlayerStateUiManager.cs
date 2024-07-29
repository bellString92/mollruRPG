using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateUiManager : MonoBehaviour
{
    public static PlayerStateUiManager Instance;
    public Transform weaponSlot;
    public Transform armorSlot;
    public Transform acceSlot;
    StateUiSlot slot;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public void SetSlot(GameObject item, ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.weaponItem:
                item.transform.SetParent(weaponSlot);
                item.transform.localPosition = Vector2.zero;
                slot = weaponSlot?.GetComponent<StateUiSlot>();
                slot.SetChild(item);
                break;
            case ItemType.armorItem:
                item.transform.SetParent(armorSlot);
                item.transform.localPosition = Vector2.zero;
                slot = armorSlot?.GetComponent<StateUiSlot>();
                slot.SetChild(item);
                break;
            case ItemType.acceItem:
                item.transform.SetParent(acceSlot);
                item.transform.localPosition = Vector2.zero;
                slot = acceSlot?.GetComponent<StateUiSlot>();
                slot.SetChild(item);
                break;
        }


        item.transform.localPosition = Vector3.zero;
    }
}
