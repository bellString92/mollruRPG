using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public delegate bool CheckAction<T>(T v);

public class Movement : AnimatorProperty
{
    public Transform myArrow;
    public float moveSpeed = 1.0f;    
    public float rotSpeed = 180.0f;
    
    protected Coroutine coMove = null;
    Coroutine coRotate = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(moveDist > 0.0f)
        {
            float delta = moveSpeed * Time.deltaTime;
            if (delta > moveDist) delta = moveDist;
            transform.Translate(moveDir * delta, Space.World);
            moveDist -= delta;
        }
        */
        /*
        if(rotAngle > 0.0f)
        {
            float delta = rotSpeed * Time.deltaTime;
            if (delta > rotAngle) delta = rotAngle;
            transform.Rotate(Vector3.up * rotDir * delta);
            rotAngle -= delta;
        }
        */
    }

    IEnumerator MovingToPos(Vector3 pos, UnityAction done)
    {
        myAnim.SetBool("IsMoving", true);
        Vector3 moveDir = pos - transform.position;
        float moveDist = moveDir.magnitude;
        moveDir.Normalize();

        coRotate = StartCoroutine(Rotating(moveDir));

        while (moveDist > 0.0f)
        {
            if (myAnim.GetBool("IsAttacking") == false)
            {
                float delta = moveSpeed * Time.deltaTime;
                if (delta > moveDist) delta = moveDist;
                transform.Translate(moveDir * delta, Space.World);
                moveDist -= delta;
            }
            yield return null;
        }
        myAnim.SetBool("IsMoving", false);
        done?.Invoke();
    }

    IEnumerator Rotating(Vector3 dir)
    {
        dir.y = 0.0f;
        dir.Normalize();
        float rotAngle = Vector3.Angle(transform.forward, dir);

        float rotDir = 1.0f;
        if (Vector3.Dot(transform.right, dir) < 0.0f)
        {
            rotDir = -1.0f;
        }

        while (rotAngle > 0.0f)
        {
            if (myAnim.GetBool("IsAttacking") == false)
            {
                float delta = rotSpeed * Time.deltaTime;
                if (delta > rotAngle) delta = rotAngle;
                transform.Rotate(Vector3.up * rotDir * delta);
                rotAngle -= delta;
            }
            yield return null;
        }
    }

    protected void StopMoveCoroutine()
    {
        if (coMove != null)
        {
            StopCoroutine(coMove);
            coMove = null;
        }
        if (coRotate != null)
        {
           StopCoroutine(coRotate);
            coRotate = null;
        }
        myAnim.SetBool("IsMoving", false);
    }

    public Coroutine MoveToPos(Vector3 pos)
    {
        return MoveToPos(pos, null);
    }

    public Coroutine MoveToPos(Vector3 pos, UnityAction done)
    {
        //StopAllCoroutines();
        StopMoveCoroutine();
        return coMove = StartCoroutine(MovingToPos(pos, done));        

        /*
        float d = Vector3.Dot(transform.forward, moveDir);
        float r = Mathf.Acos(d);
        float angle = 180.0f * (r / Mathf.PI);
        
        rotAngle = Vector3.Angle(transform.forward, moveDir);

        rotDir = 1.0f;
        if(Vector3.Dot(transform.right, moveDir) < 0.0f)
        {
            rotDir = -1.0f;
        } 
        */
    }

    public void FollowTarget(Transform target, CheckAction<float> checkAct, 
        UnityAction act)
    {
        StopMoveCoroutine();
        coMove = StartCoroutine(FollowingTarget(target, checkAct, act));
    }

    protected virtual void UpdateTargetPos(out Vector3 dir, out float dist, Transform target)
    {
        dir = target.position - transform.position;
        dist = dir.magnitude;
        dir.Normalize();
    }

    float TargetDist(Transform target)
    {
        return Vector3.Distance(transform.position, target.position);
    }

    protected IEnumerator FollowingTarget(Transform target, CheckAction<float> checkAct, UnityAction act)
    {
        myAnim.SetBool("IsMoving", true);
        while(target != null)
        {
            UpdateTargetPos(out Vector3 dir, out float dist, target);
            float delta = 0.0f;

            if (checkAct != null && checkAct.Invoke(TargetDist(target)))
            {
                myAnim.SetBool("IsMoving", false);
                //Action
                act?.Invoke();
            }
            else if(myAnim.GetBool("IsAttacking") == false)
            {
                myAnim.SetBool("IsMoving", true);
                delta = moveSpeed * Time.deltaTime;
                if (delta > dist) delta = dist;

                transform.Translate(dir * delta, Space.World);
            }
            dir.y = 0f;
            dir.Normalize();
            float angle = Vector3.Angle(transform.forward, dir);
            float rotDir = Vector3.Dot(transform.right, dir) < 0.0f ? -1.0f : 1.0f;

            delta = rotSpeed * Time.deltaTime;
            if (delta > angle) delta = angle;

            transform.Rotate(Vector3.up * delta * rotDir);

            yield return null;
        }
        myAnim.SetBool("IsMoving", false);
    }
}
