using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Slider hpBar;
    public Slider mpBar;

    public void OnChangeHP(float v)
    {
        hpBar.value = v;
    }
    public void OnChangeMP(float v)
    {
        mpBar.value = v;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
