using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject monsterPrefab;
    public List<Transform> points = new List<Transform>();//몬스터가 출현할 위치를 저장할 List 변수

    public List<GameObject> monsterPools = new List<GameObject>();//몬스터를 미리 생성해 저장
    public int maxMonsters = 10; //오브젝트 풀에 생성할 몬스터 최대 개수
    public static GameManager instance = null;//싱글톤 인스턴스 생성
    private float createTime = 3.0f;

    private void Awake()
    {
        if (instance == null)
        {//인스터스가 할당되지 않았을경우
            instance = this;
        }
        else if (instance != this)
        {//인스턴스에 할당된 클래스의 인스턴스가 다르게 새로 생성된 클래스
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);//다른 씬으로 넘어가더라도 삭제하지 않고 유지
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateMonsterPool();
        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup").transform;

        foreach (Transform point in spawnPointGroup)
        {
            this.points.Add(point);//parent 제외하고 child의 컴포넌트만 추출
        }
        InvokeRepeating("CreateMonster", 3.0f, this.createTime);//일정한 시간 간격으로 함수 호출
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void CreateMonster()
    {
        int idx = Random.Range(0, this.points.Count);//몬스터의 불규칙한 생성 위치 산출
        //풀에서 비활성화된 오브젝트 가져오기
        GameObject monster = this.GetMonsterInPool();
        monster?.transform.SetPositionAndRotation(points[idx].position, points[idx].rotation);
        monster?.SetActive(true);
    }

    public GameObject GetMonsterInPool()
    {
        //오브젝트 풀의 처음부터 끝까지 순회
        foreach (var monster in monsterPools)
        {
            if (monster.activeSelf == false)
            {
                return monster;//비활성화 여부로 사용 가능한 몬스터 판단
            }
        }
        return null;
    }
    private void CreateMonsterPool()
    {
        for (int i = 0; i < this.maxMonsters; i++)
        {
            GameObject monster = Instantiate<GameObject>(monsterPrefab);
            //monster.name = $"Monster_{i:00}";//몬스터의 이름 지정
            monster.SetActive(false);//몬스터 비활성화
            monsterPools.Add(monster);//생성한 몬스터를 오브젝트 풀에 추가
        }
    }
}
