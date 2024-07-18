using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemQuantityCheck : MonoBehaviour
{
    public TMP_Text centerText; // 중앙 텍스트를 나타내는 TextMesh Pro 요소
    public TMP_InputField inputField; // 텍스트 입력을 위한 Input Field
    public Image backgroundImage; // 배경 이미지

    private string lastValidText; // 마지막 유효한 텍스트를 저장

    private System.Action confirmCallback; // 버튼 클릭 시 실행할 콜백 함수
    private ItemKind itemData; // 아이템 정보 저장

    public void Initialize(ItemKind item, System.Action onConfirm)
    {
        itemData = item; // 전달받은 아이템 정보

        centerText.text = itemData.quantity.ToString(); // 초기값
        inputField.text = centerText.text;

        // Input Field와 중앙 텍스트를 연동합니다.
        inputField.onEndEdit.AddListener(EndEditInputField);
        inputField.onValueChanged.AddListener(ValidateInput); // 실시간 업데이트
        centerText.text = inputField.text; // 초기 텍스트를 Input Field에서 가져옵니다.

        // Input Field를 숨기고 중앙 텍스트를 클릭할 때만 보이게 설정합니다.
        inputField.gameObject.SetActive(false);
        centerText.gameObject.SetActive(true);

        // 배경 이미지를 클릭하면 Input Field가 보이도록 설정합니다.
        backgroundImage.GetComponent<Button>().onClick.AddListener(ShowInputField);

        // Input Field의 폰트 크기를 중앙 텍스트의 폰트 크기와 동일하게 설정합니다.
        inputField.textComponent.fontSize = centerText.fontSize;

        // 초기 유효 텍스트 설정
        lastValidText = centerText.text;

        // 콜백 함수 설정
        confirmCallback = onConfirm;
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
            newQuantity = Mathf.Clamp(newQuantity, 1, itemData.maxStack);

            centerText.text = newQuantity.ToString();
            itemData.quantity = newQuantity; // 아이템 데이터의 quantity 업데이트
            lastValidText = centerText.text; // 유효한 텍스트로 업데이트
        }
        else
        {
            centerText.text = lastValidText; // 유효하지 않으면 마지막 유효 텍스트로 복구
        }


        inputField.gameObject.SetActive(false);
        centerText.gameObject.SetActive(true);
    }

    private void ValidateInput(string newText)
    {
        int number;
        if (!int.TryParse(newText, out number))
        {
            inputField.text = lastValidText; // 유효하지 않으면 마지막 유효 텍스트로 복구
        }
    }

    private bool IsPointerOverGameObject(GameObject obj) //  특정 위치에만 작용하도록 재정의
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
    public void  OnPlusText()
    {
        int currentNumber;
        if (int.TryParse(centerText.text, out currentNumber))
        {
            currentNumber++;
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
            currentNumber--;
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
