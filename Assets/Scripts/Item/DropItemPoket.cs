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
    private Vector2 localPoint; // DropPoketUI의 위치를 저장할 변수

    // Start is called before the first frame update
    void Start()
    {
        dropPoketUI = DropPoketUI.Instance;
        canvas = UIManager.Instance.canvas;
    }

    // Update is called once per frame
    void Update()
    {
        // DropPoketUI가 활성화되어 있는 동안 위치를 지속적으로 업데이트
        if (dropPoketUI.IsActive())
        {
            // DropItemPoket의 위치를 캔버스 상의 좌표로 변환
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPoint,
                canvas.worldCamera,
                out localPoint
            );

            // DropPoketUI의 위치를 설정
            dropPoketUI.GetComponent<RectTransform>().localPosition = localPoint;

            // DropPoketUI가 캔버스 영역 내에 있는지 확인
            RectTransform dropPoketRectTransform = dropPoketUI.GetComponent<RectTransform>();
            RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

            // DropPoketUI의 모서리 좌표 가져오기
            Vector3[] corners = new Vector3[4];
            dropPoketRectTransform.GetWorldCorners(corners);

            // 캔버스 경계 가져오기
            Rect canvasRect = new Rect(
                canvasRectTransform.position.x - canvasRectTransform.rect.width * canvasRectTransform.pivot.x,
                canvasRectTransform.position.y - canvasRectTransform.rect.height * canvasRectTransform.pivot.y,
                canvasRectTransform.rect.width,
                canvasRectTransform.rect.height
            );

            // 모서리 중 하나라도 캔버스 범위를 벗어나면 비활성화
            bool isOutsideCanvas = false;
            foreach (Vector3 corner in corners)
            {
                if (!canvasRect.Contains(corner))
                {
                    isOutsideCanvas = true;
                    break;
                }
            }

            // DropPoketUI가 캔버스 밖으로 나가면 비활성화
            if (isOutsideCanvas)
            {
                dropPoketUI.gameObject.SetActive(false);
            }
        }


        // F키를 눌러 UI를 활성화하는 조건
        if (Input.GetKeyDown(KeyCode.F) && !dropPoketUI.IsActive() && collectable)
        {
            if (dropPoketUI != null && canvas != null)
            {
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
