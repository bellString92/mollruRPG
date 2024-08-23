using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class SkillController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Image[] myImg = null;
    public TMPro.TMP_Text[] myLabel;
    public Transform[] slots;

    private float[] coolTime;
    public Coroutine[] coCool = null;

    public static SkillController Instance = null;

    private void Awake()
    {
        Instance = this;
    }

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
        myImg = new Image[8];
        myLabel = new TMPro.TMP_Text[8];
        slots = new Transform[8];
        coolTime = new float[8];
        coCool = new Coroutine[8];
        
        
        foreach (Transform t in transform)
        {
            slots[t.GetSiblingIndex()] = t.transform;
        }
        

        for (int i = 0; i < coCool.Length; i++)
        {
            coCool[i] = null;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySkill(int v, float t)
    {
        Initialize(v, t);
        OnUse(v);
    }

    public void Initialize(int v, float t)
    {
        coolTime[v] = t;
    }

    public void OnUse(int v)
    {
        if (coCool[v] == null)
        {
            coCool[v] = StartCoroutine(Cooling(coolTime[v], v));
        }
    }

    private void OnCombo(int v)
    {
        slots[v].GetChild(0).GetComponent<PlayerSkill>().comboSkill.gameObject.SetActive(true);
        slots[v].GetChild(0).GetComponent<PlayerSkill>().comboSkill.gameObject.transform.SetParent(slots[v]);
        slots[v].GetChild(0).gameObject.SetActive(false);
    }

    private void OffCombo(int v)
    {
        slots[v].GetChild(0).gameObject.SetActive(true);
        slots[v].GetChild(1).gameObject.SetActive(false);
        slots[v].GetChild(1).transform.SetParent(slots[v].GetChild(0));
    }

    IEnumerator Cooling(float t, int i)
    {
        slots[i].GetChild(0).GetComponent<Image>().raycastTarget = false;
        if (Mathf.Approximately(t, 0.0f)) t = 1.0f;
        myImg[i].fillAmount = 1.0f;
        float curTime = 0.0f;
        while (curTime < t)
        {
            curTime += Time.deltaTime;
            myImg[i].fillAmount = 1 - (curTime / t);
            float temp = t - curTime;
            myLabel[i].text = temp > 1.0f ? ((int)temp).ToString() : temp.ToString("0.00");
            yield return null;
        }
        myImg[i].fillAmount = 0.0f;
        myLabel[i].text = "";
        
        coCool[i] = null;
        slots[i].GetChild(0).GetComponent<Image>().raycastTarget = true;
    }

    

    public PlayerSkill SlotSkillPlay(Skill skill)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetComponent<ItemSkillSlot>().slotType.Equals(SlotType.SlotSkill))
            {
                if (slots[i].GetComponentInChildren<PlayerSkill>() != null && skill.Equals(slots[i].GetComponentInChildren<PlayerSkill>().skill))
                {
                    if (coCool[i] == null || !slots[i].GetChild(0).gameObject.activeSelf)
                        return slots[i].GetComponentInChildren<PlayerSkill>();
                }
            }
        }
        return null;
    }

    public void SlotSkillOnCombo(Skill skill)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetComponent<ItemSkillSlot>().slotType.Equals(SlotType.SlotSkill))
            {
                if (slots[i].GetComponentInChildren<PlayerSkill>() != null && skill.Equals(slots[i].GetComponentInChildren<PlayerSkill>().skill))
                {
                    OnCombo(i);
                    return;
                }
            }
        }
    }

    public void SlotSkillOffCombo(Skill skill)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetComponent<ItemSkillSlot>().slotType.Equals(SlotType.SlotSkill))
            {
                if (slots[i].GetComponentInChildren<PlayerSkill>() != null && skill.Equals(slots[i].GetComponentInChildren<PlayerSkill>().skill))
                {
                    OffCombo(i);
                    return;
                }
            }
        }
    }

}
