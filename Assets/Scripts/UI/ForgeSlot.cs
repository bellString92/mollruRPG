using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeSlot : MonoBehaviour
{
    public GameObject myChild = null;
    // Start is called before the first frame update
    void Start()
    {
        IChildObject child = GetComponentInChildren<IChildObject>();
        if (child != null)
        {
            myChild = child.gameObject;
        }
    }

}
