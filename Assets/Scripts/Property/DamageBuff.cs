using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class DamageBuff : BuffSystem
{
    private float attackIncrease;


    public DamageBuff(float v, GameObject target, float t) : base(t, target)
    {
        this.attackIncrease = v;
    }
        
    public override void ApplyEffect()
    {
        Player mytarget = target.GetComponent<Player>();

        if (mytarget != null)
        {
            mytarget.myStat.AttackPoint += attackIncrease;
        }
    }

    public override void RemoveEffect()
    {
        Player mytarget = target.GetComponent<Player>();

        if (mytarget != null)
        {
            mytarget.myStat.AttackPoint -= attackIncrease;
        }
    }
}


