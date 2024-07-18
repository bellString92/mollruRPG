using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemQuantityCheck : MonoBehaviour
{
    public TMP_Text centerText; // �߾� �ؽ�Ʈ�� ��Ÿ���� TextMesh Pro ���
    public TMP_InputField inputField; // �ؽ�Ʈ �Է��� ���� Input Field
    public Image backgroundImage; // ��� �̹���

    private string lastValidText; // ������ ��ȿ�� �ؽ�Ʈ�� ����

    private System.Action confirmCallback; // ��ư Ŭ�� �� ������ �ݹ� �Լ�
    private ItemKind itemData; // ������ ���� ����

    public void Initialize(ItemKind item, System.Action onConfirm)
    {
        itemData = item; // ���޹��� ������ ����

        centerText.text = itemData.quantity.ToString(); // �ʱⰪ
        inputField.text = centerText.text;

        // Input Field�� �߾� �ؽ�Ʈ�� �����մϴ�.
        inputField.onEndEdit.AddListener(EndEditInputField);
        inputField.onValueChanged.AddListener(ValidateInput); // �ǽð� ������Ʈ
        centerText.text = inputField.text; // �ʱ� �ؽ�Ʈ�� Input Field���� �����ɴϴ�.

        // Input Field�� ����� �߾� �ؽ�Ʈ�� Ŭ���� ���� ���̰� �����մϴ�.
        inputField.gameObject.SetActive(false);
        centerText.gameObject.SetActive(true);

        // ��� �̹����� Ŭ���ϸ� Input Field�� ���̵��� �����մϴ�.
        backgroundImage.GetComponent<Button>().onClick.AddListener(ShowInputField);

        // Input Field�� ��Ʈ ũ�⸦ �߾� �ؽ�Ʈ�� ��Ʈ ũ��� �����ϰ� �����մϴ�.
        inputField.textComponent.fontSize = centerText.fontSize;

        // �ʱ� ��ȿ �ؽ�Ʈ ����
        lastValidText = centerText.text;

        // �ݹ� �Լ� ����
        confirmCallback = onConfirm;
    }

    private void Update()
    {
        // �Է� �ʵ尡 Ȱ��ȭ�� ���¿����� ó��
        if (inputField.gameObject.activeSelf && inputField.isFocused)
        {
            // �ٸ� ���� Ŭ���ϰų� Enter Ű�� ������ �� �Է� ����
            if (Input.GetMouseButtonDown(0) && !IsPointerOverGameObject(inputField.gameObject))
            {
                EndEditInputField(inputField.text);
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                EndEditInputField(inputField.text);
            }
        }
    }

    private void ShowInputField()
    {
        inputField.text = centerText.text; // ���� �ؽ�Ʈ�� Input Field�� ����
        inputField.gameObject.SetActive(true);
        centerText.gameObject.SetActive(false);
        inputField.ActivateInputField(); // Input Field�� Ȱ��ȭ�Ͽ� ��Ŀ�� ����
    }

    private void EndEditInputField(string newText)
    {
        int newQuantity;
        if (int.TryParse(newText, out newQuantity))
        {
            // �Էµ� ���� maxStack�� �Ѿ�� �ʴ��� Ȯ��
            newQuantity = Mathf.Clamp(newQuantity, 1, itemData.maxStack);

            centerText.text = newQuantity.ToString();
            itemData.quantity = newQuantity; // ������ �������� quantity ������Ʈ
            lastValidText = centerText.text; // ��ȿ�� �ؽ�Ʈ�� ������Ʈ
        }
        else
        {
            centerText.text = lastValidText; // ��ȿ���� ������ ������ ��ȿ �ؽ�Ʈ�� ����
        }


        inputField.gameObject.SetActive(false);
        centerText.gameObject.SetActive(true);
    }

    private void ValidateInput(string newText)
    {
        int number;
        if (!int.TryParse(newText, out number))
        {
            inputField.text = lastValidText; // ��ȿ���� ������ ������ ��ȿ �ؽ�Ʈ�� ����
        }
    }

    private bool IsPointerOverGameObject(GameObject obj) //  Ư�� ��ġ���� �ۿ��ϵ��� ������
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject == obj)
            {
                return true;
            }
        }
        return false;
    }




    // ��ư�� ������ �޼���
    public void  OnPlusText()
    {
        int currentNumber;
        if (int.TryParse(centerText.text, out currentNumber))
        {
            currentNumber++;
            centerText.text = currentNumber.ToString();
            inputField.text = centerText.text; // Input Field ������Ʈ
            lastValidText = centerText.text; // ��ȿ�� �ؽ�Ʈ�� ������Ʈ
        }
    }

    public void OnMinusText()
    {
        int currentNumber;
        if (int.TryParse(centerText.text, out currentNumber))
        {
            currentNumber--;
            centerText.text = currentNumber.ToString();
            inputField.text = centerText.text; // Input Field ������Ʈ
            lastValidText = centerText.text; // ��ȿ�� �ؽ�Ʈ�� ������Ʈ
        }
    }

    public void OnCloseTopUI()
    {
        UIManager.Instance.CloseTopUi();
    }

    public void OnOkButton()
    {
        // Confirm �ݹ� ȣ��
        if (confirmCallback != null)
        {
            confirmCallback.Invoke();
        }

        // UI �ݱ�
        UIManager.Instance.CloseTopUi();
    }
}
