using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropItemPoket : MonoBehaviour
{
    private GameManager gameManager; 
    public List<ItemKind> dropItems;
    private bool collectable= false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponentInParent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // F키를 눌러 UI를 활성화하는 조건
        if (Input.GetKeyDown(KeyCode.G) && collectable)
        {
            gameManager.ActivateDropPocketUI(this, dropItems);
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
        gameManager.DeactivateDropPocketUI();
    }
}
