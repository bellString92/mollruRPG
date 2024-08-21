using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class BaseQuanityCheck : MonoBehaviour, IQuantityCheck
{
    public TMP_Text titleText;
    public TMP_Text centerText;
    public TMP_InputField inputField;
    public Image backgroundImage;

    protected string lastValidText;
    protected System.Action confirmCallback;
    protected ItemKind itemData;

    public Button plusButton;
    public Button minusButton;
    public Button cancelButton;
    public Button okButton;

    private void OnEnable()
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
    }
    public virtual void Initialize(ItemKind item, System.Action onConfirm, string title)
    {
        itemData = item;
        titleText.text = title;

        centerText.text = itemData.quantity.ToString();
        inputField.text = centerText.text;

        inputField.onEndEdit.AddListener(EndEditInputField);
        inputField.onValueChanged.AddListener(ValidateInput);

        inputField.gameObject.SetActive(true);
        centerText.gameObject.SetActive(false);
        inputField.ActivateInputField();

        backgroundImage.GetComponent<Button>().onClick.AddListener(ShowInputField);

        inputField.textComponent.fontSize = centerText.fontSize;
        lastValidText = centerText.text;

        confirmCallback = onConfirm;

        plusButton.onClick.AddListener(OnPlusText);
        minusButton.onClick.AddListener(OnMinusText);
        cancelButton.onClick.AddListener(OnCloseTopUI);
        okButton.onClick.AddListener(OnOkButton);
    }

    private void Update()
    {
        if (inputField.gameObject.activeSelf && inputField.isFocused)
        {
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

    protected void ShowInputField()
    {
        inputField.text = centerText.text;
        inputField.gameObject.SetActive(true);
        centerText.gameObject.SetActive(false);
        inputField.ActivateInputField();
    }

    protected abstract void EndEditInputField(string newText);

    protected abstract void ValidateInput(string newText);

    protected void OnPlusText()
    {
        int currentNumber;
        if (int.TryParse(centerText.text, out currentNumber))
        {
            currentNumber = Mathf.Clamp(currentNumber + 1, GetMinQuantity(), GetMaxQuantity());
            UpdateQuantity(currentNumber);
        }
    }

    protected void OnMinusText()
    {
        int currentNumber;
        if (int.TryParse(centerText.text, out currentNumber))
        {
            currentNumber = Mathf.Clamp(currentNumber - 1, GetMinQuantity(), GetMaxQuantity());
            UpdateQuantity(currentNumber);
        }
    }

    protected void UpdateQuantity(int newQuantity)
    {
        centerText.text = newQuantity.ToString();
        inputField.text = centerText.text;
        lastValidText = centerText.text;
        itemData.quantity = newQuantity;
    }

    protected abstract int GetMinQuantity();

    protected abstract int GetMaxQuantity();

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

    protected void OnCloseTopUI()
    {
        UIManager.Instance.CloseTopUi();
    }

    protected void OnOkButton()
    {
        confirmCallback?.Invoke();
        UIManager.Instance.CloseTopUi();
    }
}