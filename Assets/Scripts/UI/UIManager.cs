using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private Stack<GameObject> uiStack = new Stack<GameObject>(); //�Ź� ������ UI�� ������ ���
    public Canvas canvas;
    public GameObject myInven;
    public GameObject mySkillWindow;

    public GameObject itemQuantityCheckPrefab; // Quantity�� ǥ���� ������
    private GameObject currentQuantityUI = null;
    private ItemKind itemWithPendingQuantity = null;
    private System.Action quantityConfirmCallback;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        myInven.gameObject.SetActive(false);
        mySkillWindow.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && uiStack.Count > 0) // esc�� ui���� �ż���ȣ��
        {
            CloseTopUi();
        }

        //�κ��丮 ��ȣ�ۿ�
        if (Input.GetKeyDown(KeyCode.I)) // !ChatSystem.Instance.IsActive &&  ä�� �������� ����
        {
            myInven.gameObject.SetActive(!myInven.gameObject.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (myInven.gameObject.activeSelf)
            {
                myInven.gameObject.SetActive(!myInven.gameObject.activeSelf);
            }
        }

        // ��ųâ ��ȣ�ۿ�
        if (Input.GetKeyDown(KeyCode.K)) // !ChatSystem.Instance.IsActive &&  ä�� �������� ����
        {
            mySkillWindow.gameObject.SetActive(!mySkillWindow.gameObject.activeSelf);
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

    public void OpenQuantityUI(ItemKind item, System.Action onConfirm)
    {
        // ������ ���� UI�� ������ �ݱ�
        if (currentQuantityUI != null)
        {
            CloseTopUi();
        }

        // Quantity UI ����
        currentQuantityUI = ShowUI(itemQuantityCheckPrefab);
        var quantityCheck = currentQuantityUI.GetComponent<ItemQuantityCheck>();
        if (quantityCheck != null)
        {
            quantityCheck.Initialize(item, onConfirm);
        }
    }


    public GameObject ShowUI(GameObject uiPrefab) // ȣ���ڰ� �������ִ� ������ ui����
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
