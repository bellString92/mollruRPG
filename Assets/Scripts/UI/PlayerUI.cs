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
    public float curExp;
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
        Player player = myBody.GetComponent<Player>();
        if (player != null)
        {
            maxExp = player.myStat.maxExperiencePoint;
            curExp = player.myStat.curExperiencePoint;

            if (curExp >= maxExp)
            {
                player.myStat.curExperiencePoint -= player.myStat.maxExperiencePoint;
                //curExp -= maxExp;
                player.myStat.myLvevel += 1;
            }
            PercentExp = curExp / maxExp;
        }
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
