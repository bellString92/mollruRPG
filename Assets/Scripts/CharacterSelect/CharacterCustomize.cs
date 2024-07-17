using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomize : MonoBehaviour
{
    public Transform character;
    public Transform characterSlot;
    Button[] btns;
    // Start is called before the first frame update
    void Start()
    {
        btns = characterSlot.GetComponentsInChildren<Button>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JobBtnClick(int num)
    {
        if (num % 2 == 0)
        {
            character.GetChild(0).gameObject.SetActive(true);
            character.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            character.GetChild(0).gameObject.SetActive(false);
            character.GetChild(1).gameObject.SetActive(true);
        }

        Color color = btns[num].GetComponent<Image>().color;

        for (int i = 0; i < btns.Length; i++)
        {
            if (i == num) color.a = 1.0f;
            else color.a = 0.3f;
            btns[i].GetComponent<Image>().color = color;
        }

    }
}
