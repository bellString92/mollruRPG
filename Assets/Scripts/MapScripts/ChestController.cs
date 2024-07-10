using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenChest()
    {
        if (animator != null)
        {
            animator.SetTrigger("ChestOpen");
        }
    }
}
