using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MaterialItem", menuName = "Items/MaterialItem")]// Asset/create창에서 아이템을 생성시키게 할수있는 코드
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
            // 재료 아이템은 능력치에 영향을 주지 않습니다.
            quantity--; // 사용 후 갯수 감소
        }
    }
}
