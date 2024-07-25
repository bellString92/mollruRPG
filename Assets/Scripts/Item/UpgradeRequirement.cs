using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeRequirement
{
    public int startLevel;  // 구간 시작 레벨
    public int endLevel;    // 구간 종료 레벨
    public List<MaterialIncrement> materialIncrements; // 재료 증가 폭 리스트
}