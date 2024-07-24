using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffSystem
{
    protected float duration; // 버프의 지속 시간
    protected GameObject target; // 버프가 적용될 대상



    public BuffSystem(float duration, GameObject target)
    {
        this.duration = duration;
        this.target = target;
    }

    // 버프 효과를 적용하는 메서드 (추상 메서드)
    public abstract void ApplyEffect();

    // 버프 효과를 제거하는 메서드 (추상 메서드)
    public abstract void RemoveEffect();

    // 버프의 남은 지속 시간을 줄이는 메서드
    public bool UpdateDuration(float deltaTime)
    {
        duration -= deltaTime;
        return duration <= 0;
    }
}


/*
1. 스킬을 사용한다
2. 스킬 사용후 함수를 호출한다?
3. 호출 된 함수가 버프효과를 적용한다.
4. 호출 된 함수가 쿨타임을 적용한다.
5. 쿨타임이 끝나면 함수를 종료한다.
*/