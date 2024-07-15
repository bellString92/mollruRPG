using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Q : StateMachineBehaviour
{
    Vector3 myPos;              // ���� ��ġ �����
    Transform myTarget;         // ��ǥ �����
    float moveSpeed = 2.0f;     // �̵� �ӵ�
    float distance = 3.0f;      // Ÿ�ٰ��� �Ÿ�

    private Vector3 targetPosition;
    private bool initialized = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Ÿ�� ����
        if (FieldOfView.visibleTargets.Count > 0)
        {
            myTarget = FieldOfView.visibleTargets[0];
        }
        else
        {
            myTarget = null;
            return;
        }
        //���� ��ġ ����
        myPos = animator.transform.parent.position;



        Vector3 positionTarget = myTarget.position;                 // myTarget�� ��ġ��
        Quaternion rotationTarget = myTarget.rotation;              // myTarget�� ȸ����
        Vector3 offset = rotationTarget * Vector3.right * distance; // 90�� ���� ����

        targetPosition = positionTarget + offset;           // ��ǥ ��ġ�� ���
        initialized = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (initialized)
        {
            myPos = Vector3.Lerp(myPos, targetPosition, Time.deltaTime * moveSpeed); // ��ǥ ��ġ�� �̵�
            animator.transform.parent.position = myPos; // ���� ��ġ�� ����
            animator.transform.parent.LookAt(myTarget.transform); //  Ÿ���� �ٶ�
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsSkill_Q", false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
