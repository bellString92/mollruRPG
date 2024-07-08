using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


public class Player : AnimatorProperty, IBattle
{
    private bool isNearButton = false;
    private ButtonController currentButton;

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

    //타겟 저장 변수 선언
    Transform myTarget;

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
        if (FieldOfView.visibleTargets.Count > 0) 
        {
            Transform myTarget = FieldOfView.visibleTargets[0]; // 실시간 타겟 저장
        }
        
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

        if (isNearButton && Input.GetKeyDown(KeyCode.F))
        {
            currentButton.OnButtonPress();
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
        StartCoroutine(rush(10.0f, 10.0f));
    }

    IEnumerator rush(float dirSpeed, float moveSpeed)
    {
        while (!myAnim.GetBool("myState"))
        {
            if (!myTarget) yield break; // 타겟이 비어있으면 하지 빠져나감
            Vector3 myTDir = FieldOfView.visibleTargets[0].position - transform.position; // 타겟과의 거리 계산
            float myTDist = myTDir.magnitude; // 
            float delta = 0.0f;

            if (myTarget != null)
            {
            
                delta = moveSpeed * Time.deltaTime; //프레임당 이동 거리?
                if (delta > myTDist) delta = myTDist; // 넘어가지 않게 하기 위해 델타값 변경
                transform.Translate(myTDir * delta, Space.World); // 실제 이동
            }
            else
            {
                break;
            }
            float angle = Vector3.Angle(transform.forward, myTDir);
            float rotDir = Vector3.Dot(transform.right, myTDir) < 0.0f ? -1.0f : 1.0f;

            delta = dirSpeed * Time.deltaTime;
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Button"))
        {
            isNearButton = true;
            currentButton = other.gameObject.GetComponent<ButtonController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Button"))
        {
            isNearButton = false;
            currentButton = null;
        }
    }
}
