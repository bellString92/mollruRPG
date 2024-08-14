using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AcceEffectType
{
    AttackBoost,
    MaxHealthBoost,
    CritChanceBoost,
    CritDamageBoost,
    SpeedBoost,
    // 추가 효과 유형
}
public enum AcceType // 부위 세분화
{
    Necklace,
    Ring,
    None
}

[CreateAssetMenu(fileName = "New AcceItem", menuName = "Items/AcceItem")]// Asset/create창에서 아이템을 생성시키게 할수있는 코드
public class AcceItem : ItemKind
{
    public AcceType AcceType;
    public List<AcceEffectValueList> effectList; // 여러 효과 유형

    private void OnEnable()
    {
        itemTag = "AccessoryItem";
        itemType = ItemType.acceItem;
        maxStack = 1;
    }

    public AcceItem(AcceItem original) : base(original)
    {
        AcceType = original.AcceType;
        effectList = new List<AcceEffectValueList>(original.effectList);
    }

    public override void Use(Player user) //사용시 player의 능력치에 영향을 주는 코드
    {
        AcceEffect.ApplyEffects(user, this);
    }
    public override void TakeOff(Player user)
    {
        AcceEffect.RemoveEffects(user, this);
    }

}
