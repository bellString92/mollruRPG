using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public GameObject myBody;
    public Slider hpBar;
    public Slider mpBar;
    public TMPro.TMP_Text myLevel;
    public TMPro.TMP_Text myExp;
    public float maxExp;
    public float minExp;
    public float PercentExp;

    public void OnChangeHP(float v)
    {
        hpBar.value = v;
    }
    public void OnChangeMP(float v)
    {
        mpBar.value = v;
    }

    public void OnLevel()
    {
        myLevel.text = myBody.GetComponent<Player>().myStat.myLvevel.ToString();
    }

    public void OnAddExp()
    {
        maxExp = myBody.GetComponent<Player>().myStat.maxExperiencePoint;
        minExp = myBody.GetComponent<Player>().myStat.curExperiencePoint;
        PercentExp = minExp / maxExp;
    }

    public void OnExp()
    {
        OnAddExp();
        myExp.text = PercentExp.ToString("0.00"+"%");
    }

    // Start is called before the first frame update
    void Start()
    {
      
    }
    
    void Update()
    {
        OnLevel();
        OnExp();
    }
}
