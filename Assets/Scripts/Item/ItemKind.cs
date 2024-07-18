using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType // 아이템 분류 장비,소비,재료
{

    weaponItem,
    armorItem,
    acceItem,
    consumItem,
    materialItem
}

public abstract class ItemKind : ScriptableObject
{ // 아이템에 들어갈 내용 이름,아이템 종류, 아이템 이미지, 아이템 설명
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;
    public string description;
    public int price;  // 아이템 가격
    public int resellprice; // 판매 가격
    public int itemID; // 아이템 ID

    public int maxStack;
    public int quantity;

    public ItemKind(ItemKind original)
    {
        itemName = original.itemName;
        itemIcon = original.itemIcon;
        description = original.description;
        price = original.price;
        resellprice = original.resellprice;
        quantity = original.quantity;
    }

    // abstract를 활용해 각 아이템을 use시 다른 기능을 구현되도록 함 override로 새로써 기능을 구현
    public abstract void Use(BattleStat myStat); 
}
