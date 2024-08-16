using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropItemPoket : MonoBehaviour
{
    public DropPoketUI dropPoketUI;
    public Canvas canvas;
    public List<ItemKind> dropItems;

    private bool collectable= false;

    // Start is called before the first frame update
    void Start()
    {
        dropPoketUI = DropPoketUI.Instance;
        canvas = UIManager.Instance.canvas;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !dropPoketUI.IsActive()&& collectable)
        {
            if (dropPoketUI != null && canvas != null)
            {
                // DropItemPoket의 위치를 캔버스 상의 좌표로 변환
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    screenPoint,
                    canvas.worldCamera,
                    out Vector2 localPoint
                );

                // DropPoketUI의 위치를 설정
                dropPoketUI.GetComponent<RectTransform>().localPosition = localPoint;

                // DropPoketUI를 활성화
                dropPoketUI.gameObject.SetActive(true);
                dropPoketUI.SetSlots(dropItems, this.gameObject);
            }
        }
    }

    public void GetItemList(List<ItemKind> items)
    {
        dropItems = items;
    }
    public void EmptyPoket()
    {
        gameObject.SetActive(false);
    }

    public void Collectable()
    {
        collectable = true;
    }
    public void CantCollectable()
    {
        collectable = false;    
        if(dropPoketUI != null)
        {
            dropPoketUI.gameObject.SetActive(false);
        }
    }
}
