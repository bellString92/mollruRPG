using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPocket : MonoBehaviour
{
    public Transform parent;
    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = Resources.Load<GameObject>("Prefabs/DropPocket");
        Instantiate(obj, parent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
