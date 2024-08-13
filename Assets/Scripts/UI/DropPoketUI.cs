using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropPoketUI : MonoBehaviour
{
    public static DropPoketUI Instance;
    public List<ItemKind> itemlist;
    public Transform inven;

    private GridLayoutGroup gridLayoutGroup;
    private RectTransform parentRectTransform;
    private const int maxColumns = 4;  // 한 줄에 최대 4개

    public GameObject itemBody;
    private bool isTransferring = false;

    //테스트 변수
    bool onetime = false;
    public Enemy TestMon;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        parentRectTransform = GetComponent<RectTransform>();
        AdjustSize();
    }

    void Update()
    {
        if (TestMon != null && onetime == false)
        {
            TestMon.myState.GetType();
            if (TestMon.myState == Enemy.State.Death)
            {
                itemlist = TestMon.GetRandomDropItems();
                SetSlots();
                onetime = true;
            }
        }

        // F 키를 누르면 아이템을 인벤토리에 추가하고 슬롯을 비활성화
        if (Input.GetKeyDown(KeyCode.F) && !isTransferring)
        {
            isTransferring = true;
            StartCoroutine(TransferItemsSequentially());
        }

        // F 키를 떼면 전송 중지
        if (Input.GetKeyUp(KeyCode.F))
        {
            Debug.Log("keyup");
            StopCoroutine(TransferItemsSequentially());
            isTransferring = false;
        }
    }

    public void SetSlots()
    {
        // Step 1: 모든 자식을 비활성화
        for (int i = 0; i < inven.childCount; i++)
        {
            inven.GetChild(i).gameObject.SetActive(false);
        }

        int slotIndex = 0; // 슬롯 인덱스

        // Step 2: 아이템 리스트를 순회하며 아이템을 슬롯에 할당
        for (int itemIndex = 0; itemIndex < itemlist.Count; itemIndex++)
        {
            ItemKind currentItem = itemlist[itemIndex];
            bool merged = false;

            // consumItem 또는 materialItem 타입의 경우 중복 검사
            if (currentItem.itemType == ItemType.consumItem || currentItem.itemType == ItemType.materialItem)
            {
                // 이미 존재하는 슬롯들에서 같은 itemID를 가진 아이템이 있는지 검사
                for (int j = 0; j < slotIndex; j++)
                {
                    DropPoketSlot existingSlot = inven.GetChild(j).GetComponent<DropPoketSlot>();
                    if (existingSlot.setitem.itemType == currentItem.itemType)
                    {
                        if (existingSlot.setitem.itemID == currentItem.itemID)
                        {
                            existingSlot.AddQuantity(currentItem.quantity); // 수량 합치기
                            existingSlot.UpdateQuanity(); // 수량 업데이트
                            merged = true;
                            break;
                        }
                    }
                }
            }

            // 아이템이 병합되지 않은 경우 새로운 슬롯에 추가
            if (!merged)
            {
                DropPoketSlot slot = inven.GetChild(slotIndex).GetComponent<DropPoketSlot>();
                if (slot != null)
                {
                    slot.setitem = currentItem; // 리스트의 순서대로 아이템 할당
                    slot.UpdateQuanity(); // 수량 업데이트
                    inven.GetChild(slotIndex).gameObject.SetActive(true); // 자식 활성화
                    slotIndex++; // 슬롯 인덱스 증가
                }
            }
        }

        // 아이템이 모두 할당된 후 필요에 따라 UI를 업데이트
        AdjustSize(); // 크기 조정
    }

        private void AdjustSize()
    {
        gridLayoutGroup = inven.GetComponent<GridLayoutGroup>();
        if (gridLayoutGroup == null) return;

        // Grid Layout Group에서 셀 크기와 간격을 동적으로 가져옴
        Vector2 cellSize = gridLayoutGroup.cellSize;
        Vector2 spacing = gridLayoutGroup.spacing;

        // 활성화된 자식만 세기
        int activeChildCount = 0;
        foreach (Transform child in gridLayoutGroup.transform)
        {
            if (child.gameObject.activeSelf)
            {
                activeChildCount++;
            }
        }

        // 활성화된 자식 개수에 따라 필요한 행 수 계산
        int rowCount = Mathf.CeilToInt((float)activeChildCount / maxColumns);

        // 현재 행에서 최대 열 개수 결정 (한 줄에 최대 4개, 그러나 셀이 적을 경우 해당 숫자만큼)
        int currentMaxColumns = Mathf.Min(maxColumns, activeChildCount);

        // 부모의 너비와 높이 계산
        float width = (cellSize.x + spacing.x) * currentMaxColumns - spacing.x + 20.0f;
        float height = (cellSize.y + spacing.y) * rowCount - spacing.y + 20.0f;

        // 부모 객체의 크기 조정
        parentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        parentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        // Grid Layout Group 설정 업데이트
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = maxColumns;
    }
    private void TransferItemToInventory(int slotIndex)
    {
        if (slotIndex < inven.childCount)
        {
            DropPoketSlot slot = inven.GetChild(slotIndex).GetComponent<DropPoketSlot>();
            if (slot != null && slot.setitem != null)
            {
                Inventory.Instance.CreateItem(slot.setitem, itemBody); // 인벤토리에 아이템 추가
                slot.setitem = null; // 슬롯 비우기
                slot.UpdateQuanity(); // 수량 텍스트 업데이트
                inven.GetChild(slotIndex).gameObject.SetActive(false); // 슬롯 비활성화
            }

            // 모든 슬롯이 비활성화되었는지 확인
            CheckAllSlotsDeactivated();
        }
    }

    private IEnumerator TransferItemsSequentially()
    {
        for (int i = 0; i < inven.childCount; i++)
        {
            DropPoketSlot slot = inven.GetChild(i).GetComponent<DropPoketSlot>();
            if (slot != null && slot.setitem != null)
            {
                Inventory.Instance.CreateItem(slot.setitem, itemBody); // 인벤토리에 아이템 추가
                slot.setitem = null; // 슬롯 비우기
                slot.UpdateQuanity(); // 수량 텍스트 업데이트
                inven.GetChild(i).gameObject.SetActive(false); // 슬롯 비활성화

                // 모든 슬롯이 비활성화되었는지 확인
                CheckAllSlotsDeactivated();
            }
            yield return new WaitForSeconds(0.1f); // 슬롯마다 약간의 지연 추가
        }
    }

    private void CheckAllSlotsDeactivated()
    {
        bool allSlotsDeactivated = true;
        for (int i = 0; i < inven.childCount; i++)
        {
            if (inven.GetChild(i).gameObject.activeSelf)
            {
                allSlotsDeactivated = false;
                break;
            }
        }

        if (allSlotsDeactivated)
        {
            gameObject.SetActive(false); // DropPoketUI 비활성화
        }
    }

}
