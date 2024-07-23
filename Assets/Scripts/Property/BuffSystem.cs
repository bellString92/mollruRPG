using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


interface IBeneficialBuff
{

}
interface IInjuriousBuff
{

}

interface IBuffSystem : IBeneficialBuff, IInjuriousBuff
{

}

[System.Serializable]
public struct Buff
{

}

    public class BuffSystem : BattleSystem
{
    public Sprite DamageImage;

    public void GoUpIndamage(float v, float t) // v 는 증가량 , t는 시간 
    {
        GameObject newImageObject = new GameObject("AttackBuff");   // 새로운 오브젝트 생성
        Image newImage = newImageObject.AddComponent<Image>();      // 이미지 저장
        newImage.sprite = DamageImage;                              // 스프라이트 설정

        RectTransform rectTransform = newImageObject.GetComponent<RectTransform>();
        rectTransform.SetParent(this.transform);                    // 현재 스크립트가 붙어있는 오브젝트를 부모로 설정
        rectTransform.localPosition = Vector3.zero;                 // 위치 초기화 (필요에 따라 조정 가능)
        rectTransform.sizeDelta = new Vector2(100, 100);            // 크기 설정 (필요에 따라 조정 가능)



        myBattleStat.AttackPoint += v;   // 대미지를 올린뒤
        StartCoroutine(Activation(t));       // 지속시간 체크
        
    }
    
    IEnumerator Activation(float t)
    {
        while (t > 0)        // 지속시간이 있으면
        {
            t -= 0.1f;       // 지속시간을 감소시키고
            yield return null;
        }   
        t = 0;               // 끝나면 확실하게 0으로 만든다
    }


}



// 플레이어에서 무언가를 호출하면 (상승량 , 지속시간 전달)
// 호출된 무언가가 이미지를 가져오고 
// 가져온 이미지의 필 값을 조절하면서
// 실제 대미지 값을 바꾸면서
// 지속시간동안 점차 필을 내리다가
// 지속시간이 끝나면 가져온 이미지를 지워야 한다.




