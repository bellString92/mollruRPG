using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Skill_1 : StateMachineBehaviour
{
    Transform myTarget;
    float myFrame;
    float moveSpeed = 2.0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Ÿ�� ���� �ý���
        if (FieldOfView.visibleMonster.Count > 0)
        {
            myTarget = FieldOfView.visibleMonster[0];
        }
        else  // Ÿ���� ������ Ʈ������ �ʱ�ȭ �� Ż��
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
        

        //�ִϸ��̼� �Ÿ� ����
        if (myTarget)
        {
            Vector3 myTDir = myTarget.position - animator.transform.position; // Ÿ�ٰ��� �Ÿ� ���
            float myTDist = myTDir.magnitude; // ??
            Vector3 targetPosition = myTarget.position + myTarget.forward * 2.0f; // ����� ���� ��� (��)
            myFrame = stateInfo.length; // ���� ������ ���?
            float delta = moveSpeed * myFrame * Time.deltaTime; //�����Ӵ� �̵� �Ÿ�?

            if (delta > myTDist) delta = myTDist; // �Ѿ�� �ʰ� �ϱ� ���� ��Ÿ�� ����

            animator.transform.parent.position = Vector3.Lerp(animator.transform.parent.position, targetPosition, delta); // ���� �̵�

            animator.transform.forward = (myTarget.position - animator.transform.position).normalized; // ���� ������ �׻� Ÿ������ ����
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