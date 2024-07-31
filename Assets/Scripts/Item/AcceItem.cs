using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AcceItem", menuName = "Items/AcceItem")]// Asset/create창에서 아이템을 생성시키게 할수있는 코드
public class AcceItem : ItemKind
{
    public float attackBoost;
    public float maxHealBoost;

    private void OnEnable()
    {
        itemTag = "AccessoryItem";
        itemType = ItemType.acceItem;
        maxStack = 1;
    }

    public AcceItem(AcceItem original) : base(original)
    {
        attackBoost = original.attackBoost;
        maxHealBoost = original.maxHealBoost;
    }

    public override void Use(Player user) //사용시 player의 능력치에 영향을 주는 코드
    {
        user.myStat.AttackPoint += attackBoost;
        user.myStat.maxHealPoint += maxHealBoost;
        user.myStat.curHealPoint += maxHealBoost;
    }
    public override void TakeOff(Player user)
    {
        user.myStat.AttackPoint -= attackBoost;
        user.myStat.maxHealPoint -= maxHealBoost;
        user.myStat.curHealPoint -= maxHealBoost;
    }

}
