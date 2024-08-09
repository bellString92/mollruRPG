using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Q : StateMachineBehaviour
{
    Vector3 myPos;              // 나의 위치 저장용
    Transform myTarget;         // 목표 저장용
    float moveSpeed = 2.0f;     // 이동 속도
    float distance = 3.0f;      // 타겟과의 거리

    private Vector3 targetPosition;
    private bool initialized = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 타겟 저장
        if (FieldOfView.visibleMonster.Count > 0)
        {
            myTarget = FieldOfView.visibleMonster[0];
        }
        else
        {
            myTarget = null;
            return;
        }
        // 나의 위치 저장
        myPos = animator.transform.parent.position;

        // 위치값 계산
        Vector3 positionTarget = myTarget.position;                 // myTarget의 위치값
        Quaternion rotationTarget = myTarget.rotation;              // myTarget의 회전값
        Vector3 offset = rotationTarget * Vector3.right * distance; // 90도 옆의 방향

        targetPosition = positionTarget + offset;                   // 목표 위치를 계산
        initialized = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (initialized)
        {
            myPos = Vector3.Lerp(myPos, targetPosition, Time.deltaTime * moveSpeed); // 목표 위치로 이동
            animator.transform.parent.position = myPos; // 나의 위치를 갱신
            animator.transform.parent.LookAt(myTarget.transform); //  타겟을 바라봄
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
