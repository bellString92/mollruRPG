using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private Stack<GameObject> uiStack= new Stack<GameObject>(); //�Ź� ������ UI�� ������ ���
    public Canvas canvas;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && uiStack.Count > 0) // esc�� ui���� �ż���ȣ��
        {
            CloseTopUi();
        }
    }
    public void CloseTopUi() // �������� �����ϴ� UI����
    {
        if(uiStack.Count > 0)
        {
            GameObject topUI = uiStack.Pop();
            Destroy(topUI);
        }
    }

    public GameObject ShowUI(GameObject uiPrefab) 
    { 
        if (canvas != null) 
        { 
            GameObject uiInstance = Instantiate(uiPrefab, canvas.transform); 
            uiStack.Push(uiInstance); 
            return uiInstance;
        } 
        else 
        { 
            Debug.LogError("Canvas�� �������� ����");
            return null;
        } 
    }
}
