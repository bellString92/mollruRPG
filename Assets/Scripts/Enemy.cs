using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : BattleSystem
{
    public float myExp = 100;

    public enum State
    {
        Create, Normal, Battle, Death
    }
    public State myState = State.Create;
    Vector3 startPos;
    Coroutine coRoam = null;    

    protected override void OnDead()
    {
        OnChangeState(State.Death);
    }

    void StopRoamCoroutine()
    {
        if (coRoam != null)
        {
            StopCoroutine(coRoam);
            coRoam = null;
        }
    }
    void OnChangeState(State s)
    {
        if (myState == s) return;
        myState = s;
        switch(myState)
        {
            case State.Create:                
                break;
            case State.Normal:
                StopMoveCoroutine();
                StopRoamCoroutine();                
                coRoam = StartCoroutine(Roaming());
                break;
            case State.Battle:
                StopMoveCoroutine();
                StopRoamCoroutine();
                FollowTarget(myTarget, v => v < myBattleStat.AttackRange, OnAttack);
                break;
            case State.Death:
                giveExp(myExp);
                StopAllCoroutines();
                deadAct?.Invoke();
                break;
        }
    }    

    IEnumerator Roaming()
    {
        yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
        Vector3 dir = Vector3.forward;
        dir = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0) * dir;
        dir = dir * Random.Range(1.0f, 5.0f);
        Vector3 pos = startPos + dir;
        MoveToPos(pos, () =>
        {
            StopRoamCoroutine();
            coRoam = StartCoroutine(Roaming());
        });
    }

    void StateProcess()
    {
        switch (myState)
        {
            case State.Create:
                break;
            case State.Normal:
                break;
            case State.Battle:
                BattleUpdate();
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        OnChangeState(State.Normal);
    }

    // Update is called once per frame
    void Update()
    {
        StateProcess();
    }

    public void giveExp(float exp)
    {
        if (myTarget== null) return;
        OnGiveExp(myExp);
    }
    

    public void OnBattle(Transform target)
    {
        myTarget = target;
        OnChangeState(State.Battle);
    }

    public void OnNormal()
    {
        myTarget = null;
        OnChangeState(State.Normal);
    }
    
    public void OnDisApear()
    {
        StartCoroutine(DisApearing(0.3f));
    }

    IEnumerator DisApearing(float downSpeed)
    {
        yield return new WaitForSeconds(2.0f);
        Vector3 dir = Vector3.down;
        float dist = 1.0f;
        while(dist > 0.0f)
        {
            float delta = downSpeed * Time.deltaTime;
            transform.Translate(dir * delta);
            dist -= delta;
            yield return null;
        }
        Destroy(gameObject);
    }
}
