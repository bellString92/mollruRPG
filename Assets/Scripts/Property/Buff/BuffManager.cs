using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    private List<BuffSystem> activeBuffs = new List<BuffSystem>();


    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;

        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            if (activeBuffs[i].UpdateDuration(Time.deltaTime))
            {
                activeBuffs[i].RemoveEffect();
                activeBuffs.RemoveAt(i);
            }
        }
    }

    // 버프를 추가하는 메서드
    public void AddBuff(BuffSystem buff)
    {
        // 동일한 타입의 버프가 이미 있는지 확인
        for (int i = 0; i < activeBuffs.Count; i++)
        {
            if (activeBuffs[i].GetType() == buff.GetType())
            {
                // 기존 버프의 출처와 새로운 버프의 출처를 비교
                if (activeBuffs[i].Source == buff.Source)
                {
                    // 동일한 출처일 경우 기존 버프를 제거하고 새로운 버프로 대체
                    activeBuffs[i].RemoveEffect();
                    activeBuffs.RemoveAt(i);
                    break;
                }
            }
        }
        buff.ApplyEffect();
        activeBuffs.Add(buff);
    }
}
