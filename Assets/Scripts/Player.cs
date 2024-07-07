using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Player : AnimatorProperty, IBattle
{
    public UnityEvent<float> changeHpAct;
    public BattleStat myStat;
    public LayerMask enemyMask;
    Vector2 inputDir = Vector2.zero;
    Vector2 desireDir = Vector2.zero;

    bool IsComboCheck = false;

    // 더블 클릭 체크용
    public float myDoubleClickSecond = 0.25f;
    private bool myIsOneClick = false;
    private double myTimer = 0;

    

    //public SkillIcon mySkillicon;

    public bool IsLive
    {
        get => true;
    }

    public void TakeDamage(float dmg)
    {
        myStat.curHealPoint -= dmg;
        changeHpAct?.Invoke(myStat.GetHpValue());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (ChatSystem.Instance.IsActive) return;

        desireDir.x = Input.GetAxis("Horizontal");
        desireDir.y = Input.GetAxis("Vertical");

        inputDir = Vector2.Lerp(inputDir, desireDir, Time.deltaTime * 10.0f);

        myAnim.SetFloat("x", inputDir.x);
        myAnim.SetFloat("y", inputDir.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            myAnim.SetTrigger("Roll");
        }

        if (myAnim.GetBool("myState") && Input.GetMouseButton(0))
        {
            myAnim.SetTrigger("OnAttack");
        }
        
        if (myAnim.GetBool("myState") && Input.GetKeyDown(KeyCode.Alpha1))
        {
            myAnim.SetTrigger("Skill_1");
        }

        // 더블 클릭 체크
        if (myIsOneClick && ((Time.time - myTimer) > myDoubleClickSecond))
        {
            myIsOneClick = false;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!myIsOneClick)
            {
                myTimer = Time.time;
                myIsOneClick = true;
            }
            else if (myIsOneClick && ((Time.time - myTimer) < myDoubleClickSecond))
            {
                myIsOneClick = false;
                myAnim.SetBool("Run", true);
            }
        }

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.LeftShift))
        {
            myAnim.SetBool("Run", false);
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            myAnim.SetBool("Run", true);
        }




    }

    public void OnAttack()
    {
        Collider[] list = Physics.OverlapSphere(transform.position + transform.forward * 0.5f, 0.5f, enemyMask);
        foreach(Collider col in list)
        {
            IDamage id = col.GetComponent<IDamage>();
            if(id != null)
            {
                id.TakeDamage(myStat.AttackPoint);
            }
        }
    }

    public void Skil_1()
    {
        //Transform Target = FieldOfView.visibleTargets[0];
        StartCoroutine(rush(10.0f));
    }

    IEnumerator rush(float s)
    {
        Transform Target = FieldOfView.visibleTargets[0];
        while (!myAnim.GetBool("myState"))
        {
            Vector3 myTDir = FieldOfView.visibleTargets[0].position - transform.position;
            float myTDist = myTDir.magnitude;
            //myTDir.Normalize;
            float delta = 0.0f;
            float rushSpeed = 10.0f;

            if (Target != null)
            {
            
                delta = s * Time.deltaTime;
                if (delta > myTDist) delta = myTDist;
                transform.Translate(myTDir * delta, Space.World);
            }
            else
            {
                break;
            }
            float angle = Vector3.Angle(transform.forward, myTDir);
            float rotDir = Vector3.Dot(transform.right, myTDir) < 0.0f ? -1.0f : 1.0f;

            delta = rushSpeed * Time.deltaTime;
            if (delta > angle) delta = angle;

            transform.Rotate(Vector3.up * delta * rotDir);

            yield return null;
        }
    }


    public void ComboCheckStart()
    {
        myAnim.ResetTrigger("OnAttack");
        StartCoroutine(ComboCheck());
    }

    IEnumerator ComboCheck()
    {
        myAnim.SetBool("IsCombo", true);
        IsComboCheck = true;
        while (IsComboCheck)
        {
            if (Input.GetMouseButton(0))
            {
                myAnim.SetBool("IsCombo", false);
                IsComboCheck = false;
            }
            yield return null;
        }
    }

    public void ComboCheckEnd()
    {
        IsComboCheck = false;
        myAnim.SetBool("IsCombo", true);
    }

    public void SkillStart()
    {
        myAnim.SetBool("myState", false);
    }

    public void SkillEnd()
    {
        myAnim.ResetTrigger("OnAttack");
        myAnim.SetBool("myState", true);
    }
}
