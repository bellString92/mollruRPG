using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SellQuantityCheck : MonoBehaviour
{
    public TMP_Text titleText; // 타이틀 텍스트를 나타내는 TextMesh Pro 요소
    public TMP_Text centerText; // 중앙 텍스트를 나타내는 TextMesh Pro 요소
    public TMP_InputField inputField; // 텍스트 입력을 위한 Input Field
    public Image backgroundImage; // 배경 이미지

    private string lastValidText; // 마지막 유효한 텍스트를 저장

    private System.Action confirmCallback; // 버튼 클릭 시 실행할 콜백 함수
    private ItemKind itemData; // 아이템 정보 저장

    public Button plusButton;
    public Button minusButton;
    public Button cancelButton;
    public Button okButton;

    public void Initialize(ItemKind item, System.Action onConfirm)
    {
        // 컴포넌트들을 동적으로 찾기
        titleText = transform.Find("BG/Titletext").GetComponent<TMP_Text>();
        centerText = transform.Find("BG/QuantityBG/curText").GetComponent<TMP_Text>();
        inputField = transform.Find("BG/QuantityBG/InputField (TMP)").GetComponent<TMP_InputField>();
        backgroundImage = transform.Find("BG/QuantityBG").GetComponent<Image>();

        plusButton = transform.Find("BG/Quantity++Button").GetComponent<Button>();
        minusButton = transform.Find("BG/Quantity--Button").GetComponent<Button>();
        cancelButton = transform.Find("BG/CancelButton").GetComponent<Button>();
        okButton = transform.Find("BG/OKButton").GetComponent<Button>();

        itemData = item; // 전달받은 아이템 정보

        titleText.text = "판매 수량";

        centerText.text = itemData.quantity.ToString(); // 초기값
        inputField.text = centerText.text;

        // Input Field와 중앙 텍스트를 연동합니다.
        inputField.onEndEdit.AddListener(EndEditInputField);
        inputField.onValueChanged.AddListener(ValidateInput); // 실시간 업데이트        

        // Input Field를 숨기고 중앙 텍스트를 클릭할 때만 보이게 설정합니다.
        inputField.gameObject.SetActive(true);
        centerText.gameObject.SetActive(false);
        inputField.ActivateInputField(); // Input Field를 활성화하여 포커스 설정

        // 배경 이미지를 클릭하면 Input Field가 보이도록 설정합니다.
        backgroundImage.GetComponent<Button>().onClick.AddListener(ShowInputField);

        // Input Field의 폰트 크기를 중앙 텍스트의 폰트 크기와 동일하게 설정합니다.
        inputField.textComponent.fontSize = centerText.fontSize;

        // 초기 유효 텍스트 설정
        lastValidText = centerText.text;

        // 콜백 함수 설정
        confirmCallback = onConfirm;

        // 버튼에 메서드 연결
        plusButton.onClick.AddListener(OnPlusText);
        minusButton.onClick.AddListener(OnMinusText);
        cancelButton.onClick.AddListener(OnCloseTopUI);
        okButton.onClick.AddListener(OnOkButton);
    }

    private void Update()
    {
        // 입력 필드가 활성화된 상태에서만 처리
        if (inputField.gameObject.activeSelf && inputField.isFocused)
        {
            // 다른 곳을 클릭하거나 Enter 키가 눌렸을 때 입력 종료
            if (Input.GetMouseButtonDown(0) && !IsPointerOverGameObject(inputField.gameObject))
            {
                EndEditInputField(inputField.text);
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                EndEditInputField(inputField.text);
            }
        }

        if (Input.GetKeyUp(KeyCode.Return))
        {
            OnOkButton();
        }

    }

    private void ShowInputField()
    {
        inputField.text = centerText.text; // 현재 텍스트를 Input Field로 복사
        inputField.gameObject.SetActive(true);
        centerText.gameObject.SetActive(false);
        inputField.ActivateInputField(); // Input Field를 활성화하여 포커스 설정
    }

    private void EndEditInputField(string newText)
    {
        int newQuantity;
        if (int.TryParse(newText, out newQuantity))
        {
            // 입력된 값이 maxStack을 넘어서지 않는지 확인
            newQuantity = Mathf.Clamp(newQuantity, 0, itemData.quantity);

            centerText.text = newQuantity.ToString();
            lastValidText = centerText.text; // 유효한 텍스트로 업데이트
            inputField.text = centerText.text; // 입력 필드도 업데이트
        }
        else
        {
            centerText.text = lastValidText; // 유효하지 않으면 마지막 유효 텍스트로 복구
            inputField.text = lastValidText;
        }

        inputField.gameObject.SetActive(false);
        centerText.gameObject.SetActive(true);
    }

    private void ValidateInput(string newText)
    {
        int number;
        if (string.IsNullOrEmpty(newText))
        {
            // 빈 문자열일 경우 유효하지 않은 입력으로 처리
            inputField.text = "";
        }
        else if (int.TryParse(newText, out number))
        {
            if (number > itemData.quantity)
            {
                // 입력 값이 maxStack을 넘는 경우, maxStack으로 설정
                inputField.text = itemData.quantity.ToString();
                centerText.text = itemData.quantity.ToString();
            }
            else
            {
                lastValidText = newText; // 유효한 경우 lastValidText를 업데이트
                centerText.text = newText;
            }
        }
        else
        {
            inputField.text = lastValidText; // 유효하지 않으면 마지막 유효 텍스트로 복구
        }
    }

    private bool IsPointerOverGameObject(GameObject obj)
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

    // 버튼에 연결할 메서드
    public void OnPlusText()
    {
        int currentNumber;
        if (int.TryParse(centerText.text, out currentNumber))
        {
            currentNumber = Mathf.Clamp(currentNumber + 1, 0, itemData.quantity);
            centerText.text = currentNumber.ToString();
            inputField.text = centerText.text; // Input Field 업데이트
            lastValidText = centerText.text; // 유효한 텍스트로 업데이트
        }
    }

    public void OnMinusText()
    {
        int currentNumber;
        if (int.TryParse(centerText.text, out currentNumber))
        {
            currentNumber = Mathf.Clamp(currentNumber - 1, 0, itemData.quantity);
            centerText.text = currentNumber.ToString();
            inputField.text = centerText.text; // Input Field 업데이트
            lastValidText = centerText.text; // 유효한 텍스트로 업데이트
        }
    }

    public void OnCloseTopUI()
    {
        UIManager.Instance.CloseTopUi();
    }

    public void OnOkButton()
    {
        // Confirm 콜백 호출
        if (confirmCallback != null)
        {
            confirmCallback.Invoke();
        }

        // UI 닫기
        UIManager.Instance.CloseTopUi();
    }
}