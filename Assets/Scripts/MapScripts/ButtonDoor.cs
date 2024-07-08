using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDoor : AnimatorProperty
{
    public Transform Player;
    public float interactionRange = 5.0f;
    //private float fDestroyTime = 2.0f;
    //private float fTickTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

        if (distanceToPlayer < interactionRange && Input.GetKeyDown(KeyCode.F))
        {
            myAnim.SetTrigger("Button");
            //fTickTime += Time.deltaTime;
            myAnim.SetTrigger("Open");
            /*if (fTickTime >= fDestroyTime)
            {
                myAnim.SetTrigger("Open");
            }*/
        }
    }
}
