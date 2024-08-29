using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : MySingleTon<ObjectPool>
{
    public Dictionary<string, Queue<GameObject>> myPool = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        base.Initialize();
    }

    public GameObject Instantiate<T>(GameObject org, Transform parent)
    {
        string key = typeof(T).ToString();
        if (myPool.ContainsKey(key))
        {
            if (myPool[key].Count > 0)
            {
                GameObject obj = myPool[key].Dequeue();
                obj.SetActive(true);
                obj.transform.SetParent(parent);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                return obj;
            }
        }

        return Object.Instantiate(org, parent);
    }

    public void Release<T>(GameObject obj)
    {
        obj.transform.SetParent(transform);
        obj.SetActive(false);

        string key = typeof(T).ToString();
        if (!myPool.ContainsKey(key))
        {
            myPool[key] = new Queue<GameObject>();
        }

        myPool[key].Enqueue(obj);
    }

}
