using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private Animator animator;
    public DoorController linkedDoor; // 버튼과 연결된 문

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnButtonPress()
    {
        if (animator != null)
        {
            animator.SetTrigger("Button");
        }

        if (linkedDoor != null)
        {
            linkedDoor.OpenDoor();
        }
    }
}
