using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private Stack<GameObject> uiStack= new Stack<GameObject>(); //매번 생성될 UI를 저장할 장소
    public Canvas canvas;
    public Inventory myInven;


    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        myInven.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && uiStack.Count > 0) // esc로 ui종료 매서드호출
        {
            CloseTopUi();
        }

        //인벤토리 상호작용
        if (Input.GetKeyDown(KeyCode.I)) // !ChatSystem.Instance.IsActive &&  채팅 생겼을때 쓸것
        {
            myInven.gameObject.SetActive(!myInven.gameObject.activeSelf);
        }
    }
    public void CloseTopUi() // 가장위에 존재하는 UI종료
    {
        if(uiStack.Count > 0)
        {
            GameObject topUI = uiStack.Pop();
            Destroy(topUI);
        }
    }

    public GameObject ShowUI(GameObject uiPrefab) // 호출한 npc가 가지고있는 프리펩 ui생성
    { 
        if (canvas != null) 
        { 
            GameObject uiInstance = Instantiate(uiPrefab, canvas.transform); 
            uiStack.Push(uiInstance); 
            return uiInstance;
        } 
        else 
        { 
            Debug.LogError("Canvas가 설정되지 않음");
            return null;
        } 
    }
}
