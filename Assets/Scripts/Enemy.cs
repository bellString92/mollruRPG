using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BattleSystem
{
    public Transform dropparent;
    public float myExp = 100;
    public List<DropItem> dropitemList; // 드랍할 아이템 리스트
    public int minDropQuanity; // 한번에 드롭 가능한 최소 수량
    public int maxDropQuanity; // 한번에 드롭 가능한 최대 수량

    private bool isDie; // 몬스터 생존 여부
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

        // 코루틴을 시작하기 전에 게임 오브젝트가 활성화되어 있는지 확인
        if (!gameObject.activeSelf)
        {
            return; // 게임 오브젝트가 비활성화되어 있으면 코루틴 시작을 생략
        }

        switch (myState)
        {
            case State.Create:
                GetComponent<Rigidbody>().useGravity = true;
                break;
            case State.Normal:
                GetComponent<Rigidbody>().useGravity = true;
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
                GetComponent<Rigidbody>().useGravity = false;
                giveExp(myExp);
                OnDeath(); // 죽었을때 전리품 드롭
                deadAct?.Invoke();
                StopAllCoroutines();
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
        Rigidbody rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        StateProcess();
    }

    public void giveExp(float exp)
    {
        if (myTarget == null) return;
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
        this.isDie = true;
        yield return new WaitForSeconds(3.0f);

        if (!gameObject.activeSelf)
        {
            yield break; // 게임 오브젝트가 비활성화되어 있으면 코루틴을 종료
        }

        Vector3 dir = Vector3.down;
        float dist = 1.0f;
        while (dist > 0.0f)
        {
            float delta = downSpeed * Time.deltaTime;
            transform.Translate(dir * delta);
            dist -= delta;
            yield return null;
        }

        // Destroy(gameObject); // 게임 오브젝트를 삭제하거나 비활성화
        this.myBattleStat.curHealPoint = 100;
        this.isDie = false;
        GetComponent<CapsuleCollider>().enabled = true; // 몬스터 Collider 활성화
        this.gameObject.SetActive(false);
    }

    // 아이템 드롭 메서드
    public List<ItemKind> GetRandomDropItems()
    {
        List<ItemKind> droppedItems = new List<ItemKind>();
        float totalChance = 0f;

        // 전체 확률의 합을 구함
        foreach (DropItem dropItem in dropitemList)
        {
            totalChance += dropItem.dropChance;
        }
        // 몬스터에서 지정한 최소 수량과 최대 수량 사이에서 랜덤한 수량을 선택
        int dropQuantity = Random.Range(minDropQuanity, maxDropQuanity + 1);
        // 해당 수량만큼 아이템을 리스트에 추가
        for (int i = 0; i < dropQuantity; i++)
        {
            // 0부터 전체 확률까지의 랜덤 값을 생성
            float randomValue = Random.Range(0f, totalChance);
            float cumulativeChance = 0f;

            // 랜덤 값이 어느 아이템의 범위에 속하는지 체크
            foreach (DropItem dropItem in dropitemList)
            {
                cumulativeChance += dropItem.dropChance;

                if (randomValue <= cumulativeChance)
                {
                    droppedItems.Add(dropItem.itemKind);
                    break;
                }
            }
        }
        return droppedItems;
    }

    public void OnDeath()
    {
        // 몬스터 사망 시 DropPocket 생성
        GameManager.instance.AddPocketAtPosition(transform.position);
    }
    public void SetState(State newState) 
    {
        // 몬스터 리스폰해서 활성화 시 상태를 변경
        myState = newState;
        OnChangeState(myState);
    }
}
