using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New MaterialItem", menuName = "Items/MaterialItem")]// Asset/create창에서 아이템을 생성시키게 할수있는 코드
public class MaterialItem : ItemKind
{
    private void OnEnable()
    {
        itemTag = "MaterialItem";
        itemType = ItemType.materialItem;
    }
    public MaterialItem(MaterialItem original) : base(original)
    {

    }

    public override void Use(Player myStat)
    {
        if (quantity > 0)
        {
            // 재료 아이템은 능력치에 영향을 주지 않습니다.
            quantity--; // 사용 후 갯수 감소
        }
    }
    public override void TakeOff(Player myStat)
    {
    }
}
