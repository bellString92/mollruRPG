using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

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
    public GameObject myBody; // 어딘가에 쓰이긴 하는데 어디에 쓰이는질 모르겠네;;;

    // 더블 클릭 체크용
    public float myDoubleClickSecond = 0.25f;
    private bool myIsOneClick = false;
    private double myTimer = 0;

    //타겟 저장 변수 선언
    public List<Transform> myTargetmonster = new List<Transform>();
    public List<Transform> myTarger = new List<Transform>();

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

    private bool Critical(float Cri)
    {
        float Critical = Random.Range(0f, 100f);
        if (Critical < Cri) return true;
        else return false;
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
    // Start is called before the first frame update
    void Start()
    {
        string playCharacter = PlayerPrefs.GetString("playCharacter");
        if ("".Equals(playCharacter)) playCharacter = "Paladin";
        Animator[] characters = transform.GetComponentsInChildren<Animator>();

        switch (playCharacter)
        {
            case "Paladin":
                characters[0].gameObject.SetActive(true);
                characters[1].gameObject.SetActive(false);
                break;
            case "Maria":
                characters[0].gameObject.SetActive(false);
                characters[1].gameObject.SetActive(true);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        myTargetmonster.Clear();       // 매번 초기화 해서 리스트에 아무것도 없이함
        myTarger.Clear();

        // 타겟 저장 시스템
        FieldOfView myFOV = GetComponent<FieldOfView>();
        if (myFOV.visibleMonsterView.Count > 0)
        {
            for (int i = 0; i < myFOV.visibleMonsterView.Count; i++)
            {
                myTargetmonster.Add(myFOV.visibleMonsterView[i]);
                float distance = Vector3.Distance(transform.position, myTargetmonster[0].position);        // 타겟과 나의 거리 구함, 추후 스킬과 연계하여 사용하기 위해 미리 제작
            }
        }
        if (myFOV.visibleNPCView.Count > 0)
        {
            for (int i = 0; i < myFOV.visibleNPCView.Count; i++)
            {
                myTarger.Add(myFOV.visibleNPCView[i]);
            }
        }
        if (myFOV.visibleETCView.Count > 0)
        {
            for (int i = 0;i < myFOV.visibleETCView.Count; i++)
            {
                myTarger.Add(myFOV.visibleETCView[i]);
            }
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
        if ((myTarger != null) && isNearButton && Input.GetKeyDown(KeyCode.F))
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
                OnAllSkillTrue();
                myAnim.SetTrigger("OnAttack");
            }
        }
        
        // 스킬 Q (이동기)
        if (!myAnim.GetBool("IsSkill_Q") && Input.GetKey(KeyCode.Q) && TGDir.magnitude < 3)
        {
            OnAllSkillTrue();
            myAnim.SetTrigger("OnSkill_Q");
        }

        // 스킬 E (이동기)
        if (!myAnim.GetBool("IsSkill_E") && Input.GetKey(KeyCode.E) && TGDir.magnitude < 3)
        {
            OnAllSkillTrue();
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

        // 더블 클릭 타이머 리셋
        if (myIsOneClick && (Time.time - myTimer) >= myDoubleClickSecond)
        {
            myIsOneClick = false;
        }

        // 스킬 F1
        if (!myAnim.GetBool("IsSkill_F") && Input.GetKeyDown(KeyCode.F))
        {
            OnAllSkillTrue();
            myAnim.SetTrigger("OnSkill_F");
        }

        // 스킬 1
        if (!myAnim.GetBool("IsSkill_Tab") && Input.GetKeyDown(KeyCode.Tab))
        {
            OnAllSkillTrue();
            myAnim.SetTrigger("OnSkill_Tab");
        }

        // 스킬 1
        if (!myAnim.GetBool("IsSkill_1") && Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnAllSkillTrue();
            myAnim.SetTrigger("OnSkill_1");
        }
        
        // 스킬 2
        if (!myAnim.GetBool("IsSkill_2") && Input.GetKey(KeyCode.Alpha2))
        {
            OnAllSkillTrue();
            myAnim.SetTrigger("OnSkill_2");
        }

        // 스킬 3
        if (!myAnim.GetBool("IsSkill_3") && Input.GetKeyDown(KeyCode.Alpha3))
        {
            OnAllSkillTrue();
            myAnim.SetTrigger("OnSkill_3");
        }

    }

    public void OnAllSkillTrue()
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
        myAnim.SetBool("IsSkill_F", true);
    }

    public void OnSkillReSet()
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
        myAnim.SetBool("IsSkill_F", true);
    }

    public void OnComboCheck()
    {
        myAnim.SetBool("IsSkill_F", false);
    }

    public void OnDamage()
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
    
    public void OnSkillDamage(float v)
    {
        if (myTargetmonster.Count > 0)
        {
            float distance = Vector3.Distance(transform.position, myTargetmonster[0].position);        // 타겟과 나의 거리 구함
            if (myStat.AttackRange > distance)          // 타겟과 나의 거리가 사거리보다 작을때 = 사거리안에 있을때
            {
                foreach (Transform target in myTargetmonster)      // 반복문 실행 
                {
                    IDamage id = target.GetComponent<IDamage>();    // 인터페이스 대미지 컴포넌트 가져옴
                    if (id != null)
                    {
                        if (Critical(myStat.CriticalProbability))            // 치명타 확인
                        {
                            id.TakeDamage(v * myStat.CriticalDamage);  // 치명타가 참일때 대미지에 치명타 피해량을 곱하여 피해를 입힘
                        }
                        else
                        {
                            id.TakeDamage(v);                          // 치명타가 트루일때 대미지만큼의 피해를 입힘
                        }
                    }
                }
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
