using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Skill
{
    Skill_1, Skill_2, Skill_3, Skill_Tab, Skill_F1, Skill_F2, Skill_S, Skill_Q, Skill_E
}

public class PlayerSkill : MonoBehaviour
{
    public Skill skill;
    public float CollTime;
}
