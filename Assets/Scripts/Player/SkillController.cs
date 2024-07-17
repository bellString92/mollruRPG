using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SkillController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Image[] myImg;
    UnityAction useAct;
    float coolTime = 0.0f;
    Coroutine coCool = null;
    public TMPro.TMP_Text[] myLabel;

    public void OnPointerEnter(PointerEventData eventData)
    {

    }
    public void OnPointerExit(PointerEventData eventData)
    {

    }
    public void OnPointerDown(PointerEventData eventData)
    {

    }
    public void OnPointerUp(PointerEventData eventData)
    {

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            
            OnUse(1);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnUse(int v)
    {
        if (coCool == null)
        {
            coCool = StartCoroutine(Cooling(coolTime, v));
        }
    }

    IEnumerator Cooling(float t, int i)
    {
        myLabel[i].gameObject.SetActive(true);
        if (Mathf.Approximately(t, 0.0f)) t = 1.0f;
        useAct?.Invoke();
        myImg[i].fillAmount = 0.0f;
        float curTime = 0.0f;
        while (curTime < t)
        {
            curTime += Time.deltaTime;
            myImg[i].fillAmount = curTime / t;
            float temp = t - curTime;
            myLabel[i].text = temp > 1.0f ? ((int)temp).ToString() : temp.ToString("0.00");
            yield return null;
        }
        myImg[i].fillAmount = 1.0f;
        myLabel[i].gameObject.SetActive(false);
        coCool = null;
    }

    public void Initialize(float t, UnityAction act)
    {
        coolTime = t;
        useAct = act;
    }




}
