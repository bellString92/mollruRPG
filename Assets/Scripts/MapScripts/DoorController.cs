using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : AnimatorProperty
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        if (animator != null)
        {
            animator.SetTrigger("IsOpen");
        }
    }
}
