using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcState : AnimatorProperty
{
    public enum State { Create, Nomal, Reco }
    public State mystate = State.Create;
    public Transform myTarget;
    public NpcUI myJob;
    

    void ChangeState(State s)
    {
        if (mystate == s) return;
        mystate = s;
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
    void OpenWindow()
    {

    }

}
