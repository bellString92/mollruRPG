using System.Collections.Generic;
using UnityEngine;

public class CoolTimeManager : MonoBehaviour
{
    
    [SerializeField] private Dictionary<string, SkillData> skills = new Dictionary<string, SkillData>();
    [SerializeField] public Player player;

    private void Start()
    {
        // 모든 자식 스킬 오브젝트를 찾아서 등록
        foreach (SkillData skill in GetComponentsInChildren<SkillData>())
        {
            skills[skill.skillName] = skill;
        }
    }

    public void UseSkill(string skillName)
    {
        if (skills.TryGetValue(skillName, out SkillData skill))
        {
            skill.UseSkill();
        }
    }
    
}