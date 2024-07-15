using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponItem", menuName = "Items/WeaponItem")]// Asset/createâ���� �������� ������Ű�� �Ҽ��ִ� �ڵ�
public class WeaponItem : ItemKind
{
    public float attackBoost;

    private void OnEnable()
    {
        itemType = ItemType.weaponItem;
    }

    public override void Use(BattleStat myStat) //���� player�� �ɷ�ġ�� ������ �ִ� �ڵ�
    {
        myStat.AttackPoint += attackBoost;
    }
}
