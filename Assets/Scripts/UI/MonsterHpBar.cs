using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHpBar : MonoBehaviour
{
    public Enemy monster = null;
    public bool aliveChk = true;
    public Vector3 monsterPos;
    public Vector3 hpBarPos;
    public float hpBarAlpha = 1.0f;
    public float hpBarAliveTime = 0.0f;
    public Image hpBarFill2Rect;
    private Coroutine hpBarRerfCor;

    private void Start()
    {
        gameObject.GetComponent<Slider>().value = hpBarFill2Rect.fillAmount = (monster.myBattleStat.curHealPoint / monster.myBattleStat.maxHealPoint);
        StartCoroutine(HpBarDisappear());
    }

    // Update is called once per frame
    void Update()
    {
        if (monster != null)
        {
            monsterPos = monster.transform.position;
            monsterPos.y = monsterPos.y + 2.7f;
            hpBarPos = Camera.main.WorldToScreenPoint(monsterPos);

            if (hpBarPos.z > 0)
            {
                transform.position = hpBarPos;
                gameObject.GetComponent<CanvasGroup>().alpha = hpBarAlpha;
            }
            else
            {
                gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
            }
        }
    }

    IEnumerator HpBarDisappear()
    {
        hpBarAliveTime = 0.0f;
        while (hpBarAliveTime < 5.0f)
        {
            hpBarAlpha = 1.0f;
            hpBarAliveTime += Time.deltaTime;
            if (hpBarAliveTime >= 3.0f)
            {
                hpBarAlpha = 1 - (hpBarAliveTime - 3) / 2;
            }
            yield return null;
        }
        monster.monsterHpBar = null;
        Destroy(gameObject);
    }

    public void OnHpBarChange(float curHpValue)
    {
        gameObject.GetComponent<Slider>().value = curHpValue;
        if (hpBarRerfCor != null)
        {
            StopCoroutine(hpBarRerfCor);
            hpBarRerfCor = null;
        }
        hpBarRerfCor = StartCoroutine(HpBarRerfCor());
    }

    IEnumerator HpBarRerfCor()
    {
        float dist = hpBarFill2Rect.fillAmount - gameObject.GetComponent<Slider>().value;
        float oriHp = hpBarFill2Rect.fillAmount;
        //dist = 0.2f;
        float dir = 0.0f;
        while (dir <= dist)
        {
            dir += Time.deltaTime * 2 * dist;
            hpBarFill2Rect.fillAmount = oriHp - dir;
            yield return null;
        }
        //hpBarFill2Rect.fillAmount = gameObject.GetComponent<Slider>().value;
    }
}
