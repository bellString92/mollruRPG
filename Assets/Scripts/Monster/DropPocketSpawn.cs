using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPocketSpawn : MonoBehaviour
{
    [SerializeField] GameObject dropPocketPrefab;
    [SerializeField] Transform spawnTransform;

    public List<Transform> points = new List<Transform>();//드롭포켓이 출현할 위치를 저장할 List 변수
    public List<GameObject> DropPocketPool = new List<GameObject>();//드롭포켓을 미리 생성해 저장

    public static DropPocketSpawn instance = null;//싱글톤 인스턴스 생성

    private void Awake()
    {
        if (instance == null)
        {
            //인스터스가 할당되지 않았을경우
            instance = this;
        }
        else if (instance != this)
        {
            //인스턴스에 할당된 클래스의 인스턴스가 다르게 새로 생성된 클래스
            Destroy(this.gameObject);
        }
    }

        // Start is called before the first frame update
    void Start()
    {
        CreateMonsterPool();
        Transform spawnPoint = GameObject.Find("SpawnPoint").transform;

        foreach (Transform point in spawnPoint)
        {
            points.Add(point); //parent 제외하고 child의 컴포넌트만 추출
        }
        CreateDropPocket();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateDropPocket()
    {
        int idx = Random.Range(0, points.Count);//몬스터의 불규칙한 생성 위치 산출

        //풀에서 비활성화된 오브젝트 가져오기
        GameObject dropPocket = GetDropPocketInPool();
        dropPocket?.transform.SetPositionAndRotation(points[idx].position, points[idx].rotation);
        dropPocket?.SetActive(true);
    }


    public GameObject GetDropPocketInPool()
    {
        // 오브젝트 풀의 처음부터 끝까지 순회
        foreach (var dropPocket in DropPocketPool)
        {
            if (dropPocket.activeSelf == false)
            {
                return dropPocket; // 비활성화 여부로 사용 가능한 몬스터 판단
            }
        }
        return null;
    }

    // 드롭포인트 생성
    private void CreateMonsterPool()
    {
        for (int i = 0; i < 50; i++)
        {
            GameObject dropPocket = Instantiate(dropPocketPrefab, spawnTransform); // 실제 몬스터 생성
            dropPocket.SetActive(false);   // 비활성화
            DropPocketPool.Add(dropPocket);  // 폴에 추가
        }
    }
}
