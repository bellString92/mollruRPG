using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MaterialItem", menuName = "Items/MaterialItem")]// createâ���� �������� ������Ű�� �Ҽ��ִ� �ڵ�
public class MaterialItem : ItemKind
{
    private void OnEnable()
    {
        itemType = ItemType.materialItem;
    }

    public override void Use(BattleStat myStat)
    {
        // ��� �������� �ɷ�ġ�� ������ ���� �ʽ��ϴ�.
    }
}
