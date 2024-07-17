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
       // myLevel.text = myBody.GetComponent<Player>().
    }



    // Start is called before the first frame update
    void Start()
    {
      
    }

    void Update()
    {

    }
   
     

    

}
