using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // �ߺ� ���� ����
    }

    public Transform content; // ���Ե��� �ڽ����� ������ �ִ� �θ� Transform

    // �������� �����ϴ� �޼���
    public void CreateItem(ItemKind itemKind, GameObject itemPrefab)
    {
        if (itemKind != null && itemPrefab != null)
        {
            // content�� �ڽ� ���Ե��� ��ȸ
            foreach (Transform slot in content)
            {
                InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
                if (inventorySlot != null && inventorySlot.myChild == null)
                {
                    // ItemKind�� �����Ͽ� ���
                    ItemKind newItemKind = Instantiate(itemKind);

                    // ���ο� ���� ������Ʈ ���� �� ����
                    GameObject newItem = Instantiate(itemPrefab);
                    newItem.name = newItemKind.itemName;

                    // ���ο� ���� ������Ʈ�� Drag ������Ʈ �߰�
                    newItem.AddComponent<Drag>();

                    // ������ �����ۿ� ItemComponent �߰� �� ����
                    ItemComponent itemComponent = newItem.AddComponent<ItemComponent>();
                    itemComponent.itemKind = newItemKind;

                    // ������ ����
                    Image iconImage = newItem.GetComponent<Image>();
                    if (iconImage != null && newItemKind.itemIcon != null)
                    {
                        iconImage.sprite = newItemKind.itemIcon;
                    }                  

                    // ���Կ� ������ ��ġ
                    inventorySlot.SetChild(newItem);
                    newItem.transform.SetParent(slot);

                    Debug.Log($"Created item: {newItemKind.itemName} in slot {slot.name} with independent data");

                    // ��ġ�� ũ�� ����
                    // RectTransform �ʱ�ȭ
                    RectTransform rectTransform = newItem.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.anchorMin = Vector2.zero;
                        rectTransform.anchorMax = Vector2.one;
                        rectTransform.anchoredPosition = Vector2.zero;
                        rectTransform.sizeDelta = Vector2.zero;
                        rectTransform.localPosition = Vector3.zero;
                        rectTransform.localRotation = Quaternion.identity;
                        rectTransform.localScale = Vector3.one;
                    }
                    return;

                }
            }
            Debug.LogWarning("No empty slots available.");
        }
        else
        {
            Debug.LogWarning("Invalid item kind or item prefab.");
        }
    }
}

