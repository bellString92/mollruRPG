using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageProperty : MonoBehaviour
{
    Image img = null;
    protected Image myImage
    {
        get
        {
            if (img == null)
            {
                img = GetComponentInChildren<Image>();
            }
            return img;
        }
    }
}
