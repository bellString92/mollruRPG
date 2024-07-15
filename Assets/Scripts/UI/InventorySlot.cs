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


public class InventorySlot : MonoBehaviour, IDropHandler, ISetChild
{
    public GameObject myChild = null;
    public void OnDrop(PointerEventData eventData)
    {
        if (myChild != null)
        {
            myChild.GetComponent<ISwapParent>().SwapParent(eventData.pointerDrag.GetComponent<IGetParent>().myParent);
        }
        else
        {
            eventData.pointerDrag.GetComponent<IGetParent>().myParent.GetComponent<ISetChild>().SetChild(null);
        }

        myChild = eventData.pointerDrag;
        myChild.GetComponent<IChangeParent>()?.ChangeParent(transform);
    }

    public void SetChild(GameObject newChild)
    {
        myChild = newChild;
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
