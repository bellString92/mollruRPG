using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialRequirement
{
    public ItemKind requiredItem;  // 필요한 재료 아이템
    public int quantity;  // 필요한 수량
}