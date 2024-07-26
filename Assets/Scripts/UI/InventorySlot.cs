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
    public Player User;
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

        
        myChild = eventData.pointerDrag;
        myChild.GetComponent<IChangeParent>()?.ChangeParent(transform);

    }
  
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (ShopManager.Instance != null)
            {
                if (myChild != null)
                {
                    ShopManager.Instance.SetDestroySlotItem(this);
                }
                else
                {
                    ShopManager.Instance.SetDestroySlotItem(null);
                }
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 우클릭 처리: use 실행
            if (myChild != null)
            {
                myChild.GetComponent<SaveItemInfo>()?.itemKind.Use(User.myStat);
                if (myChild.GetComponent<SaveItemInfo>()?.itemKind.quantity == 0)
                {
                    Destroy(myChild);
                }
            }
        }
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
        if (User == null)
        {
            User = Inventory.Instance.user;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
