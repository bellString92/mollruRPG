using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

interface IChildObject
{
    GameObject gameObject
    {
        get;
    }
}

interface ISetChild
{
    void SetChild(GameObject newChild);
}


public class InventorySlot : MonoBehaviour, IDropHandler, ISetChild , IPointerClickHandler
{
    public GameObject myChild = null;
    public void OnDrop(PointerEventData eventData)
    {
        if (myChild != null)
        {
            myChild.GetComponent<ISwapParent>()?.SwapParent(eventData.pointerDrag.GetComponent<IGetParent>().myParent);
        }
        else
        {
            eventData.pointerDrag.GetComponent<IGetParent>()?.myParent.GetComponent<ISetChild>().SetChild(null);
        }

        if (myChild = null) return;
        myChild = eventData.pointerDrag;
        myChild.GetComponent<IChangeParent>()?.ChangeParent(transform);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        ShopManager.Instance.SetDestroySlotItem(this); // 클릭으로 마우스 이벤트가 끝났을때 이슬롯을 선택해 아이템 삭제하기위해 정보 전달
    }

    public void SetChild(GameObject newChild)
    {
        myChild = newChild;
    }
    public void DestroyChild() // 마이차일드 비우기
    {
        if (myChild != null)
        {
            Destroy(myChild);
            myChild = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        IChildObject child = GetComponentInChildren<IChildObject>();
        if (child != null)
        {
            myChild = child.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
