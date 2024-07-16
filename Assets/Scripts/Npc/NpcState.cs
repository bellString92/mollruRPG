using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcState : AnimatorProperty
{
    public enum State { Create, Nomal, Reco }
    public State mystate = State.Create;
    public Transform myTarget;

    public GameObject myJob; // 각 npc가 담당할 Ui를 프리펩으로 만들고 바인딩
    private GameObject doMyJob; // 상호작용을 여러번해 중복 생성 되지않도록 이미 상호작용중 이라면 생성한 object를 저장해서 관리

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
            case State.Nomal:// 매번 Nomal로 돌아오면 처음 배치했던 방향으로 되돌아간다
                {
                    StopAllCoroutines();
                    StartCoroutine(RetrunbeforReco());
                }
                break;
            case State.Reco:// 플레이어를 인식했으면 플레이어 방향으로 고개를 돌린다
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
                                doMyJob = UIManager.Instance.ShowUI(myJob);// 중복생성 방지를 위한 domyjob저장 ShowUI는 정상작동
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

    IEnumerator LookPlayer() // 인식범위에 들어오면 플레이어를 향해 돌아봄
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
    IEnumerator RetrunbeforReco() // 인식범위를 벗어나면 처음 보고있던 방향으로 돌아감
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
    public void StopMyJob() // npc가 자신이 생성한 상호작용 ui종료
    {
        if(doMyJob != null)
        {
            Destroy(doMyJob);
            doMyJob = null;
        }
    }

}
