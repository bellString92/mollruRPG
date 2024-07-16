using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcState : AnimatorProperty
{
    public enum State { Create, Nomal, Reco }
    public State mystate = State.Create;
    public Transform myTarget;

    public GameObject myJob; // �� npc�� ����� Ui�� ���������� ����� ���ε�
    private GameObject doMyJob; // ��ȣ�ۿ��� �������� �ߺ� ���� �����ʵ��� �̹� ��ȣ�ۿ��� �̶�� ������ object�� �����ؼ� ����

    public Inventory urInventory;
    
    Vector3 startPos;
    Quaternion startRot;
    float moveduration = 1.0f;

    void ChangeState(State s)
    {
        if (mystate == s) return;
        mystate = s;
        switch (mystate)
        {
            case State.Create:
                break;
            case State.Nomal:// �Ź� Nomal�� ���ƿ��� ó�� ��ġ�ߴ� �������� �ǵ��ư���
                {
                    StopAllCoroutines();
                    StartCoroutine(RetrunbeforReco());
                }
                break;
            case State.Reco:// �÷��̾ �ν������� �÷��̾� �������� ���� ������
                {
                    StopAllCoroutines();
                    myAnim.SetTrigger("OnGesture");
                    StartCoroutine(LookPlayer());
                }
                break;
        }
    }
    void StateProcess()
    {
        switch (mystate)
        {
            case State.Create:
                break;
            case State.Nomal:
                {
                   
                }
                break;
            case State.Reco:
                {
                    if (FieldOfView.visibleNPC.Count > 0 && FieldOfView.visibleNPC[0] != null)
                    {
                        if (FieldOfView.visibleNPC[0].transform == transform && Input.GetKeyDown(KeyCode.F))
                        {
                            if(UIManager.Instance != null && doMyJob == null)
                            {
                                doMyJob = UIManager.Instance.ShowUI(myJob);// �ߺ����� ������ ���� domyjob���� ShowUI�� �����۵�
                                urInventory.gameObject.SetActive(true);
                            }
                        }
                    }
                }
                break;
        }
    }

    public void OnFind(Transform target)
    {
        myTarget = target;
        ChangeState(State.Reco);
    }
    public void OnNomal()
    {
        myTarget= null;
        StopMyJob();
        ChangeState(State.Nomal);
    }


    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        ChangeState(State.Nomal);
    }

    // Update is called once per frame
    void Update()
    {
        StateProcess();
    }

    IEnumerator LookPlayer() // �νĹ����� ������ �÷��̾ ���� ���ƺ�
    {
        while(myTarget!= null)
        {
            Vector3 dir = myTarget.position - transform.position;
            dir.y= 0;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3.0f); ;
            yield return null;
        }
    }
    IEnumerator RetrunbeforReco() // �νĹ����� ����� ó�� �����ִ� �������� ���ư�
    {
        Vector3 curPos = transform.position;
        Quaternion curRot = transform.rotation;
        float elapsedTime = 0.0f;
        while (elapsedTime < moveduration)
        {
            transform.position = Vector3.Lerp(curPos, startPos, elapsedTime / moveduration);
            transform.rotation = Quaternion.Slerp(curRot, startRot, elapsedTime / moveduration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;
        transform.rotation = startRot;
    }
    public void StopMyJob() // npc�� �ڽ��� ������ ��ȣ�ۿ� ui����
    {
        if(doMyJob != null)
        {
            Destroy(doMyJob);
            doMyJob = null;
        }
    }

}
