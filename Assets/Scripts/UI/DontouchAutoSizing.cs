using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontouchAutoSizing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RectTransform size = UIManager.Instance?.GetComponent<RectTransform>();       
        RectTransform mysize = this?.transform.GetComponent<RectTransform>();
        mysize.sizeDelta = size.sizeDelta;  
    }

}
