using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType // 아이템 분류 장비,소비,재료
{

    weaponItem, // 무기 아이템
    armorItem, // 방어구 "
    acceItem, // 악세사리 "
    consumItem, // 소비 "
    materialItem // 재료 "
}
// 아이템 레어등급
public enum Rarity
{
    Common,      // 일반
    Uncommon,    // 고급
    Rare,        // 희귀
    Epic,        // 영웅
    Legendary    // 전설
}

public abstract class ItemKind : ScriptableObject
{ // 아이템에 들어갈 내용 이름,아이템 종류, 아이템 이미지, 아이템 설명
    public string itemName;
    public ItemType itemType; // 어떤 종류의 아이템인지 정의
    public string itemTag; // 아이템에 태그 부착
    public Sprite itemIcon; //아이템 이미지
    public string description; // 아이템 툴팁
    public int price;  // 아이템 가격
    public int resellprice; // 판매 가격
    public int itemID; // 아이템 ID

    public int maxStack; // 최대수량
    public int quantity; // 현재 수량

    public Rarity rarity; // 아이템 레어등급
    public int kaiLevel; // 강화 수치

    public ItemKind(ItemKind original)
    {
        itemName = original.itemName;
        itemIcon = original.itemIcon;
        description = original.description;
        price = original.price;
        resellprice = original.resellprice;
        quantity = original.quantity;
        rarity = original.rarity;
        kaiLevel = original.kaiLevel;
    }

    // abstract를 활용해 각 아이템을 use시 다른 기능을 구현되도록 함 override로 새로써 기능을 구현
    public abstract void Use(BattleStat myStat); 
}
