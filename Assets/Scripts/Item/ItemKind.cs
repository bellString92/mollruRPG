using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        kaiLevel = Mathf.Min(original.kaiLevel, GetMaxKaiLevel(original.rarity));
    }

    public int KaiLevel
    {
        get => kaiLevel;
        set => kaiLevel = Mathf.Min(value, GetMaxKaiLevel(rarity));
    }

    // 아이템 등급에 따른 최대 강화 수치 반환
    public int GetMaxKaiLevel(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return 3;
            case Rarity.Uncommon:
                return 5;
            case Rarity.Rare:
                return 10;
            case Rarity.Epic:
                return 15;
            case Rarity.Legendary:
                return 20;
            default:
                return 0;
        }
    }

    // 강화 단계, 레어도에 따라 강화 확률 반환
    public float GetUpgradeSuccessRate()
    {
        switch (rarity)
        {
            case Rarity.Common:
                if (kaiLevel < 2) return 1.0f;
                if (kaiLevel < 3) return 0.9f;
                break;
            case Rarity.Uncommon:
                if (kaiLevel < 3) return 1.0f;
                if (kaiLevel < 5) return 0.8f;
                break;
            case Rarity.Rare:
                if (kaiLevel < 5) return 0.8f;
                if (kaiLevel < 8) return 0.7f;
                if (kaiLevel < 10) return 0.6f;
                break;
            case Rarity.Epic:
                if (kaiLevel < 5) return 0.8f;
                if (kaiLevel < 10) return 0.65f;
                if (kaiLevel < 15) return 0.5f;
                break;
            case Rarity.Legendary:
                if (kaiLevel < 5) return 0.75f;
                if (kaiLevel < 10) return 0.6f;
                if (kaiLevel < 15) return 0.3f;
                if (kaiLevel < 19) return 0.2f;
                if (kaiLevel < 20) return 0.1f;
                break;
        }
        return 0.0f; // 만약 범위가 아닌 경우 0%로 반환
    }
    // 각 레어도에 따른 증가 수치 반환
    public float[] GetIncrementValuesByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return new float[] { 1.1f, 1.1f, 1.1f, 1.1f }; // 예시 값
            case Rarity.Uncommon:
                return new float[] { 1.1f, 1.1f, 1.1f, 1.12f }; // 예시 값
            case Rarity.Rare:
                return new float[] { 1.1f, 1.1f, 1.1f, 1.12f }; // 예시 값
            case Rarity.Epic:
                return new float[] { 1.1f, 1.1f, 1.11f, 1.13f }; // 예시 값
            case Rarity.Legendary:
                return new float[] { 1.1f, 1.1f, 1.12f, 1.15f }; // 예시 값
            default:
                return new float[] { 0.0f, 0.0f, 0.0f, 0.0f };
        }
    }

    // abstract를 활용해 각 아이템을 use시 다른 기능을 구현되도록 함 override로 새로써 기능을 구현
    public abstract void Use(BattleStat myStat);
}
