using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType // ������ �з� ���,�Һ�,���
{

    weaponItem,
    armorItem,
    acceItem,
    consumItem,
    materialItem
}

public abstract class ItemKind : ScriptableObject
{ // �����ۿ� �� ���� �̸�,������ ����, ������ �̹���, ������ ����
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;
    public string description;
    public int price;  // ������ ����
    public int resellprice; // �Ǹ� ����
    public int itemID; // ������ ID

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

    // abstract�� Ȱ���� �� �������� use�� �ٸ� ����� �����ǵ��� �� override�� ���ν� ����� ����
    public abstract void Use(BattleStat myStat); 
}
