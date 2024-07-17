using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MaterialItem", menuName = "Items/MaterialItem")]// Asset/createâ���� �������� ������Ű�� �Ҽ��ִ� �ڵ�
public class MaterialItem : ItemKind
{
    private void OnEnable()
    {
        itemType = ItemType.materialItem;
        maxStack = 999;
    }
    public MaterialItem(MaterialItem original) : base(original)
    {

    }

    public override void Use(BattleStat myStat)
    {
        if (quantity > 0)
        {
            // ��� �������� �ɷ�ġ�� ������ ���� �ʽ��ϴ�.
            quantity--; // ��� �� ���� ����
        }
    }
}
