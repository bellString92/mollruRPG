using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeRequirement
{
    public int kaiLevel;  // 강화 단계
    public List<MaterialRequirement> materialRequirements;  // 재료 요구사항 리스트
}