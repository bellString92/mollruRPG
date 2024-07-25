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
        buff.ApplyEffect();
        activeBuffs.Add(buff);
    }
}
