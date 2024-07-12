using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Skill_move : StateMachineBehaviour
{
    Transform myTarget;
    bool myTargeton;
    float myFrame;
    Vector3 myForward;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        myForward = animator.transform.forward;


        if (FieldOfView.visibleTargets.Count > 0)
        {
            myTargeton = true;
        }
        else
        {
            myTargeton = false;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.forward = myForward;


        // 타겟 저장 시스템
        if (myTargeton && FieldOfView.visibleTargets.Count > 0) // 타겟이 있으면 트랜스폼 저장
        {
            myTarget = FieldOfView.visibleTargets[0];
        }
        else  // 타겟이 없으면 트랜스폼 초기화 및 탈출
        {
            myTarget = null;
            return;
        }
        
        //애니메이션 거리 조절
        Vector3 myTDir = myTarget.transform.position - animator.transform.position; // 타겟과의 거리 계산
        float myTDist = myTDir.magnitude; // ?? 
        float delta = 0.0f;
        myFrame = animator.GetCurrentAnimatorStateInfo(0).length; // 남은 프레임 계산?


        if (myTarget != null)
        {
            delta = myFrame * Time.deltaTime; //프레임당 이동 거리?
            
            if (delta > myTDist) delta = myTDist; // 넘어가지 않게 하기 위해 델타값 변경
            animator.transform.parent.Translate(myTDir * delta, Space.World); // 실제 이동
        }
        else
        {
            return;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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
