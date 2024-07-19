using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Player : AnimatorProperty, IBattle
{
    private bool isNearButton, isNearChest = false;
    private ButtonController currentButton;
    private ChestController currentChest;
    public UnityEvent<float> changeHpAct;
    public UnityEvent<float> changeMpAct;
    public BattleStat myStat;
    public LayerMask enemyMask;
    Vector3 TGDir;
    Vector2 inputDir = Vector2.zero;
    Vector2 desireDir = Vector2.zero;
    public GameObject myBody;


    //bool IsComboCheck = false;

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

    public void ExpSystem()
    {
        myStat.maxExperiencePoint = myStat.myLvevel * 10;
    }

    public void LevelSystem()
    {
        if (myStat.curExperiencePoint >= myStat.maxExperiencePoint)
        {
            myStat.myLvevel++;
        }
    }


    // Update is called once per frame
    void Update()
    {
        // 실시간 타겟 저장
        if (FieldOfView.visibleMonster.Count > 0) 
        {
            myTarget = FieldOfView.visibleMonster[0];
            TGDir = transform.position - myTarget.position;
        }
        else
        {
            myTarget = null;
        }

        // 이동
        desireDir.x = Input.GetAxis("Horizontal");
        desireDir.y = Input.GetAxis("Vertical");

        inputDir = Vector2.Lerp(inputDir, desireDir, Time.deltaTime * 10.0f);

        myAnim.SetFloat("x", inputDir.x);
        myAnim.SetFloat("y", inputDir.y);

        // W+W 달리기
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

        // W+LShift 달리기
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            myAnim.SetBool("Run", true);
        }

        // 달리기 해제
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.LeftShift))
        {
            myAnim.SetBool("Run", false);
        }

        // 구르기
        if (!myAnim.GetBool("IsRoll") && Input.GetKeyDown(KeyCode.Space)) 
        {
            myAnim.SetBool("IsRoll", true);
            myAnim.SetTrigger("OnRoll");
        }

        // 상호작용키
        if ((myTarget != null) && isNearButton && Input.GetKeyDown(KeyCode.F))
        {
            currentButton.OnButtonPress();
        }
        if (currentChest != null && isNearChest && Input.GetKeyDown(KeyCode.F))
        {
            currentChest.OpenChest();
        }

        // 기본공격 좌클
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (!myAnim.GetBool("IsAttack") && Input.GetMouseButton(0))
            {
                myAnim.SetBool("IsRoll", true);
                myAnim.SetBool("IsAttack", true);
                myAnim.SetTrigger("OnAttack");
            }
        }

        // 스킬 Q (이동기)
        if (!myAnim.GetBool("IsSkill_Q") && Input.GetKey(KeyCode.Q) && TGDir.magnitude < 3)
        {
            myAnim.SetBool("IsSkill_Q", true);
            myAnim.SetTrigger("OnSkill_Q");
        }

        // 스킬 E (이동기)
        if (!myAnim.GetBool("IsSkill_E") && Input.GetKey(KeyCode.E) && TGDir.magnitude < 3)
        {
            myAnim.SetBool("IsSkill_E", true);
            myAnim.SetTrigger("OnSkill_E");
        }

        // 스킬 SS (이동기)
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!myIsOneClick)
            {
                myTimer = Time.time;
                myIsOneClick = true;
            }
            else if (myIsOneClick && ((Time.time - myTimer) < myDoubleClickSecond))
            {
                myIsOneClick = false;
                myAnim.SetTrigger("OnSkill_S");
            }
        }

        // 스킬 F1
        if (!myAnim.GetBool("IsSkill_F1") && Input.GetKeyDown(KeyCode.F))
        {
            myAnim.SetBool("IsSkill_F1", true);
            myAnim.SetTrigger("OnSkill_F1");
        }

        // 스킬 1
        if (!myAnim.GetBool("IsSkill_Tab") && Input.GetKeyDown(KeyCode.Tab))
        {
            myAnim.SetBool("IsSkill_Tab", true);
            myAnim.SetTrigger("OnSkill_Tab");
        }

        // 스킬 1
        if (!myAnim.GetBool("IsSkill_1") && Input.GetKeyDown(KeyCode.Alpha1))
        {
            myAnim.SetBool("IsSkill_1", true);
            myAnim.SetTrigger("OnSkill_1");
        }
        
        // 스킬 2
        if (!myAnim.GetBool("IsSkill_2") && Input.GetKeyDown(KeyCode.Alpha2))
        {
            myAnim.SetBool("IsSkill_2", true);
            myAnim.SetTrigger("OnSkill_2");
        }

        // 스킬 3
        if (!myAnim.GetBool("IsSkill_3") && Input.GetKeyDown(KeyCode.Alpha3))
        {
            myAnim.SetBool("IsSkill_3", true);
            myAnim.SetTrigger("OnSkill_3");
        }

        // 스킬 4
        if (!myAnim.GetBool("IsSkill_4") && Input.GetKeyDown(KeyCode.Alpha4))
        {
            myAnim.SetBool("IsSkill_4", true);
            myAnim.SetTrigger("OnSkill_4");
        }

    }
    public void AllAttackTrue()
    {
        myAnim.SetBool("IsAttack", true);
    }

    public void AllAttackFalse()
    {
        myAnim.SetBool("IsAttack", false);
    }

    public void AllSkillTrue()
    {
        myAnim.SetBool("IsRoll", true);
        myAnim.SetBool("IsAttack", true);
        myAnim.SetBool("IsSkill_Q", true);
        myAnim.SetBool("IsSkill_E", true);
        myAnim.SetBool("IsSkill_S", true);
        myAnim.SetBool("IsSkill_Tab", true);
        myAnim.SetBool("IsSkill_1", true);
        myAnim.SetBool("IsSkill_2", true);
        myAnim.SetBool("IsSkill_3", true);
        myAnim.SetBool("IsSkill_4", true);
        myAnim.SetBool("IsSkill_F1", false);
        myAnim.SetBool("IsSkill_F2", false);
    }

    public void AllSkillFalse()
    {
        myAnim.SetBool("IsRoll", false);
        myAnim.SetBool("IsAttack", false);
        myAnim.SetBool("IsSkill_Q", false);
        myAnim.SetBool("IsSkill_E", false);
        myAnim.SetBool("IsSkill_S", false);
        myAnim.SetBool("IsSkill_Tab", false);
        myAnim.SetBool("IsSkill_1", false);
        myAnim.SetBool("IsSkill_2", false);
        myAnim.SetBool("IsSkill_3", false);
        myAnim.SetBool("IsSkill_4", false);
        myAnim.SetBool("IsSkill_F1", true);
        myAnim.SetBool("IsSkill_F2", true);
    }

    public void SkillCheck()
    {
        if (myAnim.GetBool("IsSkill_F1"))
        {
            myAnim.SetTrigger("OnSkill_F1");
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

    // 던전 문과 상자 제어하기 위한 클라이더
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Button"))
        {
            isNearButton = true;
            currentButton = other.gameObject.GetComponent<ButtonController>();
        }
        if (other.gameObject.CompareTag("Chest"))
        {
            isNearChest = true;
            currentChest = other.gameObject.GetComponent<ChestController>();
        }
    }

    // 던전 문과 상자 제어하기 위한 클라이더
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Button"))
        {
            isNearButton = false;
            currentButton = null;
        }
        if (other.gameObject.CompareTag("Chest"))
        {
            isNearChest = false;
            currentChest = null;
        }
    }
}
