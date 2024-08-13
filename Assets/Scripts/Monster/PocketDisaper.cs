using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketDisaper : MonoBehaviour
{
    [SerializeField] GameObject pocket;

    public float SetActiveFalseDelay;

    void Start()
    {
       
    }
    void Update()
    {
        SetActiveFalseDelay += Time.deltaTime;
        if (SetActiveFalseDelay > 90)
        {
            pocket.SetActive(false);
        }
    }

}
