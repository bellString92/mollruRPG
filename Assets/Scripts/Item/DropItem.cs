using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropItem 
{
    public ItemKind itemKind; // 드롭할 아이템 정보
    public int quanity; // 드롭할 아이템 갯수
    public float dropChance;  // 드롭 확률 (0~100)
}
