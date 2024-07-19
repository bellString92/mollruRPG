using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Skill_Tab : StateMachineBehaviour
{
    Transform myTarget;
    float myFrame;
    float moveSpeed = 2.0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 타겟 저장 시스템
        if (FieldOfView.visibleMonster.Count > 0)
        {
            myTarget = FieldOfView.visibleMonster[0];
        }
        else  // 타겟이 없으면 트랜스폼 초기화 및 탈출
        {
            myTarget = null;
            return;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsAttack", true);
        animator.SetBool("IsSkill_1", true);
        

        //애니메이션 거리 조절
        if (myTarget)
        {
            Vector3 myTDir = myTarget.position - animator.transform.position; // 타겟과의 거리 계산
            float myTDist = myTDir.magnitude; // ??
            Vector3 targetPosition = myTarget.position + myTarget.forward * 2.0f; // 대상의 방향 계산 (앞)
            myFrame = stateInfo.length; // 남은 프레임 계산?
            float delta = moveSpeed * myFrame * Time.deltaTime; //프레임당 이동 거리?

            if (delta > myTDist) delta = myTDist; // 넘어가지 않게 하기 위해 델타값 변경

            animator.transform.parent.position = Vector3.Lerp(animator.transform.parent.position, targetPosition, delta); // 실제 이동

            animator.transform.forward = (myTarget.position - animator.transform.position).normalized; // 나의 방향을 항상 타겟으로 고정
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsAttack", false);
        animator.SetBool("IsSkill_1", false);
        animator.transform.localRotation = Quaternion.identity;
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