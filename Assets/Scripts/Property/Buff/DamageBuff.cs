using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class DamageBuff : BuffSystem
{
    //public Animator _myAni;

    private float attackIncrease;
    //private GameObject myBody;
    private BuffType type;

    public DamageBuff(float v, GameObject target, float t, BuffType b, BuffSource s) : base(t, target, s)
    {
        this.attackIncrease = v;
        //this.myBody = target;
        this.type = b;
    }

    public override void ApplyEffect()
    {
        Player mytarget = target.GetComponent<Player>();

        if (mytarget != null)
        {
            switch (type)
            {
                case BuffType.Damage:
                    mytarget.myStat.AttackPoint += attackIncrease;
                    break;
                case BuffType.Defense:
                    mytarget.myStat.AttackPoint += attackIncrease;
                    break;
                case BuffType.MoveSpeed:
                    mytarget.myStat.moveSpeed += attackIncrease;
                    break;
                case BuffType.AttackSpeed:
                    mytarget.myStat.AttackRange += attackIncrease;
                    break;
                case BuffType.AttackRange:
                    mytarget.myStat.AttackRange += attackIncrease;
                    break;
            }
        }
    }

    public override void RemoveEffect()
    {
        Player mytarget = target.GetComponent<Player>();

        if (mytarget != null)
        {
            switch (type)
            {
                case BuffType.Damage:
                    mytarget.myStat.AttackPoint -= attackIncrease;
                    break;
                case BuffType.Defense:
                    mytarget.myStat.AttackPoint -= attackIncrease;
                    break;
                case BuffType.MoveSpeed:
                    mytarget.myStat.moveSpeed -= attackIncrease;
                    break;
                case BuffType.AttackSpeed:
                    mytarget.myStat.AttackPoint -= attackIncrease;
                    break;
                case BuffType.AttackRange:
                    mytarget.myStat.AttackRange -= attackIncrease;
                    break;
            }
        }
    }
}


