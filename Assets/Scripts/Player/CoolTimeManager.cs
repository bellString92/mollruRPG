using Scene_Teleportation_Kit.Scripts.player;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoolTimeManager : MonoBehaviour
{
    private static CoolTimeManager instance;
    private Dictionary<int, float> cooldowns = new Dictionary<int, float>();
    [SerializeField] private Dictionary<string, SkillData> skills = new Dictionary<string, SkillData>();

    private void Awake()
    {
        instance = this;
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
            skill.transform.parent.gameObject.SetActive(true);
            skill.UseSkill();
        }
    }


    // 아이템쿨타임 
    public static void StartCooldown(int itemId, float duration)
    {
        if (instance != null)
        {
            instance.cooldowns[itemId] = Time.time + duration;
        }
    }

    public static bool IsOnCooldown(int itemId)
    {
        if (instance != null && instance.cooldowns.ContainsKey(itemId))
        {
            return Time.time < instance.cooldowns[itemId];
        }
        return false;
    }

    public static float GetRemainingCooldown(int itemId)
    {
        if (instance != null && instance.cooldowns.ContainsKey(itemId))
        {
            return Mathf.Max(0, instance.cooldowns[itemId] - Time.time);
        }
        return 0;
    }
}