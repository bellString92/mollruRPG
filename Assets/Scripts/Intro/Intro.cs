using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    public TMPro.TMP_Text myTitle;
    public TMPro.TMP_Text myStart;
    private IEnumerator introCor = null;
    private IEnumerator startCor = null;
    private IEnumerator startWaitCor = null;

    // Start is called before the first frame update
    void Start()
    {
        myTitle.text = "";
        myStart.text = "Press Any Key to Start";
        introCor = IntroCor("MollRu RPG", 1.0f, 0.5f, 1.0f, 3, 0.5f);
        StartCoroutine(introCor);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (introCor != null) {
                StopCoroutine(introCor);
                introCor = null;
                myTitle.text = "MollRu RPG";
                startCor = StartCor(1.0f, 3, 0.5f);
                StartCoroutine(startCor);
            } else {
                StopAllCoroutines();
                introCor = null;
                startCor = null;
                PlayerPrefs.SetString("nextSceneText", "로딩 중");
                PlayerPrefs.SetString("nextSceneImage", "Dungeon");
                SceneChange.OnSceneChange("2-1. CharacterSelect");
            }
        }

    }

    /// <summary>
    /// title : 게임타이틀
    /// waitTime : 타이틀 나오기전 대기시간
    /// titleWordTime : 타이틀 글자 타이핑 속도
    /// startTime : 하단 글자 투명도 속도
    /// dot : 점 갯수
    /// dotTime : 점 속도
    /// </summary>
    IEnumerator IntroCor(string title, float waitTime, float titleWordTime, float startTime, int dot, float dotTime)
    {
        yield return new WaitForSeconds(waitTime);
        for (int i = 0; i < title.Length; i++)
        {
            yield return new WaitForSeconds(titleWordTime);
            myTitle.text += title[i];
        }
        yield return new WaitForSeconds(waitTime);

        startCor = StartCor(startTime, dot, dotTime);
        StartCoroutine(startCor);
    }

    IEnumerator StartCor(float startTime, int dot, float dotTime)
    {
        introCor = null;
        float curTime = 0.0f;
        float colorA = 0.0f;
        Color color = myStart.color;
        while (curTime < startTime)
        {
            curTime += Time.deltaTime;
            colorA = curTime * startTime;
            color.a = colorA;
            myStart.color = color;
            yield return null;
        }

        startWaitCor = StartWaitCor(dot, dotTime);
        StartCoroutine(startWaitCor);
    }

    IEnumerator StartWaitCor(int dot, float dotTime)
    {
        string oriStartTxt = myStart.text;
        string dotTxt = ".";
        while (true)
        {
            yield return new WaitForSeconds(dotTime);
            myStart.text = oriStartTxt + dotTxt;
            if (dotTxt.Length >= dot) dotTxt = "";
            else dotTxt += ".";
        }
    }
}
