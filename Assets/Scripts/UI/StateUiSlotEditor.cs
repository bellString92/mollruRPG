using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StateUiSlot))]
public class StateUiSlotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StateUiSlot slot = (StateUiSlot)target;

        // 기본 인스펙터 요소 그리기
        DrawDefaultInspector();

        // allowedItemType에 따른 추가 필드 표시 제어
        switch (slot.AllowedItemType)
        {
            case ItemType.armorItem:
                slot.AllowedArmorType = (ArmorType)EditorGUILayout.EnumPopup("Allowed Armor Type", slot.AllowedArmorType);
                break;
            case ItemType.acceItem:
                slot.AllowedAcceType = (AcceType)EditorGUILayout.EnumPopup("Allowed Accessory Type", slot.AllowedAcceType);
                break;
            default:
                // 다른 경우에는 아무것도 표시하지 않음
                break;
        }
    }
}