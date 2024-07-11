using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public string nextScene = "Intro";
    public LayerMask targetMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & targetMask) != 0)
        {
            SceneChange.OnSceneChange(nextScene);
        }
    }
}
