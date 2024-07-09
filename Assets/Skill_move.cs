using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Skill_move : StateMachineBehaviour
{
    Transform myTarget;
    bool myTargeton;
    float myFrame; 

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
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
        // Ÿ�� ���� �ý���
        if (myTargeton && FieldOfView.visibleTargets.Count > 0) // Ÿ���� ������ Ʈ������ ����
        {
            myTarget = FieldOfView.visibleTargets[0];
        }
        else  // Ÿ���� ������ Ʈ������ �ʱ�ȭ �� Ż��
        {
            myTarget = null;
            return;
        }
        
        //�ִϸ��̼� �Ÿ� ����
        Vector3 myTDir = myTarget.transform.position - animator.transform.position; // Ÿ�ٰ��� �Ÿ� ���
        float myTDist = myTDir.magnitude; // ?? 
        float delta = 0.0f;
        myFrame = animator.GetCurrentAnimatorStateInfo(0).length; // ���� ������ ���?


        if (myTarget != null)
        {
            delta = myFrame * Time.deltaTime; //�����Ӵ� �̵� �Ÿ�?
            
            if (delta > myTDist) delta = myTDist; // �Ѿ�� �ʰ� �ϱ� ���� ��Ÿ�� ����
            animator.transform.Translate(myTDir * delta, Space.World); // ���� �̵�
        }
        else
        {
            return;
        }

        //ȸ��
        float angle = Vector3.Angle(animator.transform.forward, myTDir);
        float rotDir = Vector3.Dot(animator.transform.right, myTDir) < 0.0f ? -1.0f : 1.0f;

        delta = 10.0f * Time.deltaTime;
        if (delta > angle) delta = angle;

        animator.transform.Rotate(Vector3.up * delta * rotDir);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
