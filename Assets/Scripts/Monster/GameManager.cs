using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject dropPocketPrefab;
    [SerializeField] GameObject monsterPrefab;
    [SerializeField] Transform parentTransform; // 몬스터 풀의 부모 트랜스폼
    [SerializeField] Transform pocketparentTransform; //드롭 포켓 풀의 부모 트랜스폼

    public List<Transform> points = new List<Transform>(); // 몬스터가 출현할 위치를 저장할 List 변수

    public List<GameObject> monsterPools = new List<GameObject>(); // 몬스터를 미리 생성해 저장
    public List<GameObject> pocketPools = new List<GameObject>(); // 아이템 미리 저장

    public int maxMonsters = 10; // 오브젝트 풀에 생성할 몬스터 최대 개수
    public static GameManager instance = null; // 싱글톤 인스턴스 생성
    private float createTime = 3.0f;

    private void Awake()
    {
        if (instance == null)
        {
            // 인스턴스가 할당되지 않았을 경우
            instance = this;
        }
        else if (instance != this)
        {
            // 인스턴스에 할당된 클래스의 인스턴스가 다르게 새로 생성된 경우
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateMonsterPool();
        InitializePocketPool(); // DropPocket 풀 초기화

        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup").transform;

        foreach (Transform point in spawnPointGroup)
        {
            points.Add(point); // parent 제외하고 child의 컴포넌트만 추출
        }
        InvokeRepeating("CreateMonster", 3.0f, this.createTime); // 일정한 시간 간격으로 함수 호출
    }

    private void InitializePocketPool()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject dropPocket = Instantiate(dropPocketPrefab, pocketparentTransform);
            dropPocket.SetActive(false);
            pocketPools.Add(dropPocket); // 생성된 드롭 포켓을 pocketPools에 추가
        }
    }

    private void CreateMonster()
    {
        int idx = Random.Range(0, points.Count); // 몬스터의 불규칙한 생성 위치 산출

        // 풀에서 비활성화된 오브젝트 가져오기
        GameObject monster = GetMonsterInPool();
        if (monster != null)
        {
            monster.transform.SetPositionAndRotation(points[idx].position, points[idx].rotation);
            monster.SetActive(true);
            monster.transform.GetChild(1).gameObject.SetActive(true);


            // Enemy상태를 Create로 변경
            Enemy enemyComponent = monster.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.SetState(Enemy.State.Create);
            }

        }
    }

    public GameObject GetMonsterInPool()
    {
        // 오브젝트 풀의 처음부터 끝까지 순회
        foreach (var monster in monsterPools)
        {
            if (!monster.activeSelf)
            {
                return monster; // 비활성화 여부로 사용 가능한 몬스터 판단
            }
        }
        return null;
    }

    public GameObject GetDropPocketInPool()
    {
        // 오브젝트 풀의 처음부터 끝까지 순회
        foreach (var dropPocket in pocketPools)
        {
            if (!dropPocket.activeSelf)
            {
                return dropPocket; // 비활성화 여부로 사용 가능한 드롭 포켓 판단
            }
        }
        return null;
    }

    private void CreateMonsterPool()
    {
        for (int i = 0; i < maxMonsters; i++)
        {
            GameObject monster = Instantiate(monsterPrefab, parentTransform); // 실제 몬스터 생성
            monster.SetActive(false); // 몬스터 비활성화
            monsterPools.Add(monster); // 몬스터 풀에 추가
        }
    }

    public void AddPocketAtPosition(Vector3 position)
    {
        // 오브젝트 풀에서 비활성화된 DropPocket을 가져옴
        GameObject dropPocket = GetDropPocketInPool();

        if (dropPocket != null)
        {
            // DropPocket을 활성화하고 위치를 설정
            dropPocket.transform.position = position;
            dropPocket.SetActive(true);
        }
        else
        {
            // 풀에 남아있는 DropPocket이 없으면 새로 생성
            dropPocket = Instantiate(dropPocketPrefab, position, Quaternion.identity);
            dropPocket.transform.SetParent(pocketparentTransform); // DropPocket을 parentTransform의 자식으로 설정
            pocketPools.Add(dropPocket); // 새로 생성된 클론을 pocketPools에 추가
        }
    }
}