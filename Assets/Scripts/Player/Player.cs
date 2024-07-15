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
    public BattleStat myStat;
    public LayerMask enemyMask;
    Vector3 TGDir;
    Vector2 inputDir = Vector2.zero;
    Vector2 desireDir = Vector2.zero;

    bool IsComboCheck = false;

    // ���� Ŭ�� üũ��
    public float myDoubleClickSecond = 0.25f;
    private bool myIsOneClick = false;
    private double myTimer = 0;

    //Ÿ�� ���� ���� ����
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
        /*
        // ���콺 ����ó�� ����
        GameObject go = GameObject.Find("MouseHide");
        if (go == null)
        {
            go = new GameObject("MouseHide");
            go.AddComponent<MouseHide>();
        }
        // ���콺 ����ó�� ��
        */
        // �ǽð� Ÿ�� ����
        if (FieldOfView.visibleTargets.Count > 0) 
        {
            myTarget = FieldOfView.visibleTargets[0];
            TGDir = transform.position - myTarget.position;
        }
        else
        {
            myTarget = null;
        }

        //Ÿ�ٰ� ���� �Ÿ�

        // �̵�
        desireDir.x = Input.GetAxis("Horizontal");
        desireDir.y = Input.GetAxis("Vertical");

        inputDir = Vector2.Lerp(inputDir, desireDir, Time.deltaTime * 10.0f);

        myAnim.SetFloat("x", inputDir.x);
        myAnim.SetFloat("y", inputDir.y);

        //W+W�޸���
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

        //W+LShift �޸���
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            myAnim.SetBool("Run", true);
        }

        //�޸��� ����
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.LeftShift))
        {
            myAnim.SetBool("Run", false);
        }

        //������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            myAnim.SetTrigger("Roll");
        }

        // Ui�� ��ȣ �ۿ����϶� ���콺�Է� ����
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (!myAnim.GetBool("IsAttack") && Input.GetMouseButton(0)) 
            {
                myAnim.SetTrigger("OnAttack");
            }
        }

        //��ȣ�ۿ�Ű
        if ((myTarget != null) && isNearButton && Input.GetKeyDown(KeyCode.F))
        {
            currentButton.OnButtonPress();
        }
        if (currentChest != null && isNearChest && Input.GetKeyDown(KeyCode.F))
        {
            currentChest.OpenChest();
        }



        // ���ó �ּ� ���
        if (myIsOneClick && ((Time.time - myTimer) > myDoubleClickSecond))
        {
            myIsOneClick = false;
        }

        
        
        
        

        //�⺻���� ��Ŭ
        if (!myAnim.GetBool("IsAttack") && Input.GetMouseButton(0))
        {
            myAnim.SetTrigger("OnAttack");
        }
        
        // ��ų Q (�̵���)
        if (!myAnim.GetBool("IsSkill_Q") && Input.GetKey(KeyCode.Q) && TGDir.magnitude < 3)
        {
            myAnim.SetBool("IsSkill_Q", true);
            myAnim.SetTrigger("OnSkill_Q");
        }

        // ��ų E (�̵���)
        if (!myAnim.GetBool("IsSkill_E") && Input.GetKey(KeyCode.E) && TGDir.magnitude < 3)
        {
            myAnim.SetBool("IsSkill_E", true);
            myAnim.SetTrigger("OnSkill_E");
        }

        // ��ų S (�̵���)
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

        // ��ų F1
        if (!myAnim.GetBool("IsSkill_F1") && Input.GetKeyDown(KeyCode.F))
        {
            myAnim.SetBool("IsSkill_F1", true);
            myAnim.SetTrigger("OnSkill_F1");
        }
        
        //��ų 1
        if (!myAnim.GetBool("IsSkill_1") && Input.GetKeyDown(KeyCode.Alpha1))
        {
            myAnim.SetBool("IsSkill_1", true);
            myAnim.SetTrigger("OnSkill_1");
        }
        
        //��ų 2
        if (!myAnim.GetBool("IsSkill_2") && Input.GetKeyDown(KeyCode.Alpha2))
        {
            myAnim.SetBool("IsSkill_2", true);
            myAnim.SetTrigger("OnSkill_2");
        }

        //��ų 3
        if (!myAnim.GetBool("IsSkill_3") && Input.GetKeyDown(KeyCode.Alpha3))
        {
            myAnim.SetBool("IsSkill_3", true);
            myAnim.SetTrigger("OnSkill_3");
        }

        //��ų 4
        if (!myAnim.GetBool("IsSkill_4") && Input.GetKeyDown(KeyCode.Alpha4))
        {
            myAnim.SetBool("IsSkill_4", true);
            myAnim.SetTrigger("OnSkill_4");
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

    public void Skill_Movement()
    {
        StartCoroutine(SkillMovement(10.0f, 10.0f));
    }

    IEnumerator SkillMovement(float dirSpeed, float moveSpeed)
    {   
        while (!myAnim.GetBool("myState"))
        {
            
            if (!myTarget) yield break; // Ÿ���� ��������� ���� ��������
            Vector3 myTDir = myTarget.transform.position - transform.position; // Ÿ�ٰ��� �Ÿ� ���
            float myTDist = myTDir.magnitude; // 
            float delta = 0.0f;

            if (myTarget != null)
            {
            
                delta = moveSpeed * Time.deltaTime; //�����Ӵ� �̵� �Ÿ�?
                if (delta > myTDist) delta = myTDist; // �Ѿ�� �ʰ� �ϱ� ���� ��Ÿ�� ����
                //transform.Translate(myTDir * delta, Space.World); // ���� �̵�
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


    public void AttackCheckStart()
    {
        StartCoroutine(AttackCheck());
    }

    IEnumerator AttackCheck()
    {
        myAnim.SetBool("IsSkill_2", false);
        myAnim.SetBool("IsAttack", false);
        IsComboCheck = true;
        while (IsComboCheck)
        {
            if (Input.GetMouseButton(0))
            {
                IsComboCheck = false;
            }
            yield return null;
        }
    }

    public void AttackCheckEnd()
    {
        IsComboCheck = false;
    }

    public void SkillCheckStart()
    {
        myAnim.SetBool("IsAttack", false);
    }

    public void SkillCheckEnd()
    {
        myAnim.SetBool("IsSkill_1", false);
        myAnim.SetBool("IsSkill_2", false);
        myAnim.SetBool("IsSkill_3", false);
        myAnim.SetBool("IsSkill_4", false);
    }

    // ���� ���� ���� �����ϱ� ���� Ŭ���̴�
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

    // ���� ���� ���� �����ϱ� ���� Ŭ���̴�
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
