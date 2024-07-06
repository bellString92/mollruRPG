using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AIMovement : Movement
{
    NavMeshPath myPath;
    Coroutine move = null;
    protected void DrawPath()
    {
        if (myPath != null)
        {
            for (int i = 0; i < myPath.corners.Length - 1; ++i)
            {
                Debug.DrawLine(myPath.corners[i], myPath.corners[i + 1], Color.red);
            }
        }
    }

    public new void MoveToPos(Vector3 pos)
    {
        MoveToPos(pos, null);
    }

    public new void MoveToPos(Vector3 pos, UnityAction done)
    {
        if (myPath == null) myPath = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, pos, NavMesh.AllAreas, myPath))
        {
            switch (myPath.status)
            {
                case NavMeshPathStatus.PathComplete:
                case NavMeshPathStatus.PathPartial:
                    if (move != null)
                    {
                        StopCoroutine(move);
                        move = null;
                    }
                    move = StartCoroutine(MovingByPath(myPath.corners, done));
                    break;
                case NavMeshPathStatus.PathInvalid:
                    break;
            }
        }
        else
        {
            done?.Invoke();
        }
    }

    IEnumerator MovingByPath(Vector3[] path, UnityAction done)
    {
        if (myArrow != null)
        {
            myArrow.gameObject.SetActive(true);
            myArrow.position = path[path.Length - 1];
        }
        int curIdx = 1;
        while (curIdx < path.Length)
        {
            yield return base.MoveToPos(path[curIdx++]);
        }
        if (myArrow != null) myArrow.gameObject.SetActive(false);
        done?.Invoke();
    }

    public new void FollowTarget(Transform target, CheckAction<float> checkAct,
        UnityAction act)
    {
        if(coMove != null)
        {
            StopCoroutine(coMove);
            coMove = null;
        }
        coMove = StartCoroutine(FollowingTarget(target, checkAct, act));
    }

    protected override void UpdateTargetPos(out Vector3 dir, out float dist, Transform target)
    {
        if (myPath == null) myPath = new NavMeshPath();
        dir = Vector3.zero;
        dist = 0.0f;
        NavMesh.CalculatePath(transform.position, target.position, -1, myPath);
        if(myPath.corners.Length > 1)
        {
            dir = myPath.corners[1] - transform.position;
            dist = dir.magnitude;
            dir.Normalize();
        }
    }

    /*
    IEnumerator FollowingTarget(Transform target, CheckAction<float> checkAct,
        UnityAction act)
    {
        while (target != null)
        {
            if (myPath == null) myPath = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, target.position, -1, myPath);
            if (myPath.corners.Length > 1)
            {
                Vector3 dir = myPath.corners[1] - transform.position;
                float dist = dir.magnitude, delta;
                dir.Normalize();

                if (checkAct != null && checkAct.Invoke(Vector3.Distance(transform.position, target.position)))
                {
                    myAnim.SetBool("IsMoving", false);
                    //Action
                    act?.Invoke();
                }
                else if (myAnim.GetBool("IsAttacking") == false)
                {
                    if (dist > 0.0f)
                    {
                        myAnim.SetBool("IsMoving", true);
                        delta = Time.deltaTime * moveSpeed;
                        if (delta > dist) delta = dist;
                        transform.Translate(dir * delta, Space.World);
                    }
                    else
                    {
                        myAnim.SetBool("IsMoving", false);
                    }
                }

                float angle = Vector3.Angle(transform.forward, dir);
                float rotDir = Vector3.Dot(transform.right, dir) > 0.0f ? 1.0f : -1.0f;
                if (angle > 0.0f)
                {
                    delta = Time.deltaTime * rotSpeed;
                    if (delta > angle) delta = angle;
                    transform.Rotate(Vector3.up * delta * rotDir);
                }
            }
            else
            {
                myAnim.SetBool("IsMoving", false);
            }
            yield return null;
        }
    }
    */
}
