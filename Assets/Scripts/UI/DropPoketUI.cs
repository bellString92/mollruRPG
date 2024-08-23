using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class DropPoketUI : MonoBehaviour
{
    public static DropPoketUI Instance;
    public List<ItemKind> itemlist;
    public Transform inven;

    private GridLayoutGroup gridLayoutGroup;
    private RectTransform parentRectTransform;
    private const int maxColumns = 4;  // 한 줄에 최대 4개

    public GameObject itemBody;
    private float actionCooldown = 0.1f; // 0.1초 간격
    private float timer = 0f;

    bool isRunning = false;
    GameObject openPoket;


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
        gameObject.SetActive(false);
        parentRectTransform = GetComponent<RectTransform>();
        AdjustSize();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            isRunning = false;
            gameObject.SetActive(false);
        }
        if (Input.GetKey(KeyCode.G))
        {
            timer += Time.deltaTime;  // 타이머를 증가시킴

            if (timer >= actionCooldown)  // 타이머가 간격을 초과했을 때
            {
                TransferNextItemToInventory();
                timer = 0f;  // 타이머 초기화
            }
        }
        else
        {
            timer = 0f;  // F 키가 눌리지 않으면 타이머 초기화
        }
    }

    private void OnEnable()
    {
        IsActive();
    }

    public void SetSlots(List<ItemKind> dropItems, GameObject curPoket)
    {
        itemlist = dropItems;
        openPoket = curPoket;

        // Step 1: 모든 자식을 비활성화
        for (int i = 0; i < inven.childCount; i++)
        {
            inven.GetChild(i).gameObject.SetActive(false);
        }

        int slotIndex = 0; // 슬롯 인덱스

        // 아이템 리스트를 순회하며 중복된 아이템을 제거하기 위한 리스트
        List<ItemKind> itemsToRemove = new List<ItemKind>();

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

                            // 중복된 아이템을 리스트에서 제거하기 위해 추가
                            itemsToRemove.Add(currentItem);
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

        // 병합된 아이템들을 itemlist에서 제거
        foreach (ItemKind item in itemsToRemove)
        {
            itemlist.Remove(item);
        }

        // 아이템이 모두 할당된 후 필요에 따라 UI를 업데이트
        AdjustSize(); // 크기 조정
    }

    public void AdjustSize()
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

    private void TransferNextItemToInventory()
    {
        for (int i = 0; i < inven.childCount; i++)
        {
            DropPoketSlot slot = inven.GetChild(i).GetComponent<DropPoketSlot>();
            if (slot != null && slot.setitem != null)
            {
                if (slot.setitem.itemID == 1)
                {
                    itemlist.Remove(slot.setitem);
                    int dropglod = slot.setitem.quantity;
                    Inventory.Instance.user.myStat.myGold += dropglod;
                }
                else
                {
                    // 리스트에서 아이템 제거
                    itemlist.Remove(slot.setitem);
                    Inventory.Instance.CreateItem(slot.setitem, itemBody); // 인벤토리에 아이템 추가
                }

                slot.setitem = null;
                slot.UpdateQuanity();
                inven.GetChild(i).gameObject.SetActive(false);
                AdjustSize();

                CheckAllSlotsDeactivated();
                break;  // 한 번에 하나씩만 처리
            }
        }
    }

    public void CheckAllSlotsDeactivated()
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
            openPoket.SetActive(false);
        }
    }

    public bool IsActive()
    {
        if(gameObject.activeSelf)
        {
            isRunning = true;
        }
        else if (!gameObject.activeSelf)
        {
            isRunning = false;
        }
        return isRunning;
    }
    
}
