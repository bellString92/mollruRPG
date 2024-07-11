using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcState : AnimatorProperty
{
    public enum State { Create, Nomal, Reco }
    public State mystate = State.Create;
    public Transform myTarget;
    public NpcUI myJob;
    
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
                    if (FieldOfView.visibleTargets.Count > 0 && FieldOfView.visibleTargets[0] != null)
                    {
                        if (FieldOfView.visibleTargets[0].transform == transform && Input.GetKeyDown(KeyCode.F))
                        {
                            myJob.gameObject.SetActive(true);
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
        myJob.gameObject.SetActive(false);
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

    IEnumerator LookPlayer()
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
    IEnumerator RetrunbeforReco()
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

}
