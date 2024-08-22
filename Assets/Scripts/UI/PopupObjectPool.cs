using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupObjectPool : MonoBehaviour
{
    public static PopupObjectPool Instance { get; private set; }

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        Instance = this;
    }

    public void CreatePool(string tag, GameObject prefab, int poolSize)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab,this.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary[tag] = objectPool;
        }
    }

    public GameObject GetFromPool(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"풀에 {tag}가 없습니다!");
            return null;
        }

        GameObject objectToReuse = poolDictionary[tag].Dequeue();

        objectToReuse.SetActive(true);
        poolDictionary[tag].Enqueue(objectToReuse);

        return objectToReuse;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }
}