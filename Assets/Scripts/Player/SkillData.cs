using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillData : MonoBehaviour
{
    [SerializeField] public string skillName;
    [SerializeField] public string triggerName;
    [SerializeField] public float cooldownTime;
    [SerializeField] public Player Player;


    [SerializeField] UnityAction useAct;
    [SerializeField] Coroutine coCool = null;
    [SerializeField] public Image myImg;
    [SerializeField] public TMPro.TMP_Text myLabel;

    private void Start()
    {
        Player = transform.parent.parent.parent.GetComponent<CoolTimeManager>().player;
        myImg = GetComponent<Image>();
        myLabel = transform.GetChild(1).GetComponent<TMP_Text>();
    }

    public void UseSkill()
    {
        if (coCool == null)
        {
            coCool = StartCoroutine(Cooling(cooldownTime));
            Player.myAnim.SetTrigger(triggerName);
            Debug.Log(skillName + " 성공!");
        }
        else
        {
            Debug.Log("쿨타임!");
        }
    }

    IEnumerator Cooling(float t)
    {
        myLabel.gameObject.SetActive(true);
        if (Mathf.Approximately(t, 0.0f)) t = 1.0f;
        useAct?.Invoke();
        myImg.fillAmount = 0.0f;
        float curTime = 0.0f;
        while (curTime < t)
        {
            Player.myAnim.SetBool(skillName, true);
            curTime += Time.deltaTime;
            myImg.fillAmount = curTime / t;
            float temp = t - curTime;
            //myLabel.text = temp.ToString(); //> 1.0f ? ((int)temp).ToString() : temp.ToString("0.00");
            yield return null;
        }
        myImg.fillAmount = 1.0f;
        myLabel.gameObject.SetActive(false);
        Player.myAnim.SetBool(skillName, false);
        coCool = null;
    }

}
