using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponItem", menuName = "Items/WeaponItem")]// Asset/create창에서 아이템을 생성시키게 할수있는 코드
public class WeaponItem : ItemKind
{
    public float attackBoost;

    private void OnEnable()
    {
        itemTag = "WeaponItem";
        itemType = ItemType.weaponItem;
        maxStack = 1;
    }
    public WeaponItem(WeaponItem original) : base(original)
    {
        attackBoost = original.attackBoost;
    }

    public override void Use(BattleStat myStat) //사용시 player의 능력치에 영향을 주는 코드
    {
        myStat.AttackPoint += attackBoost;
    }
}
