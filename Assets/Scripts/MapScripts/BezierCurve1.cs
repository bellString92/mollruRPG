using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BezierCurve1 : MonoBehaviour
{
    public GameObject GameObject;
    [Range(0, 1)]
    public float Test;

    public Vector3 P1;

    public Vector3 P2;

    public Vector3 P3;

    public Vector3 P4;

    private void Update()
    {
        GameObject.transform.position = BezierTest(P1, P2, P3, P4, Test);
    }
    public Vector3 BezierTest(
        Vector3 P_1,
        Vector3 P_2,
        Vector3 P_3,
        Vector3 P_4,
        float Value
        )
    {
        Vector3 A = Vector3.Lerp(P_1, P_2, Value);
        Vector3 B = Vector3.Lerp(P_2, P_3, Value);
        Vector3 C = Vector3.Lerp(P_3, P_4, Value);

        Vector3 D = Vector3.Lerp(A, B, Value);
        Vector3 E = Vector3.Lerp(B, C, Value);

        Vector3 F = Vector3.Lerp(D, E, Value);

        return F;
    }
}
[CanEditMultipleObjects]
[CustomEditor(typeof(BezierCurve))]
public class BezierCurve1_Editor : Editor
{
    private void OnSceneGUI()
    {
        BezierCurve Generator = (BezierCurve)target;

        Generator.P1 = Handles.PositionHandle(Generator.P1, Quaternion.identity);
        Generator.P2 = Handles.PositionHandle(Generator.P2, Quaternion.identity);
        Generator.P3 = Handles.PositionHandle(Generator.P3, Quaternion.identity);
        Generator.P4 = Handles.PositionHandle(Generator.P4, Quaternion.identity);

        Handles.DrawLine(Generator.P1, Generator.P2);
        Handles.DrawLine(Generator.P3, Generator.P4);
        int Count = 50;
        for(float i = 0; i < Count; i++)
        {
            float value_Before= i / 10;
            Vector3 Before = Generator.BezierTest(Generator.P1, Generator.P2, Generator.P3, Generator.P4, value_Before);
            float value_After= (i+1) / 10;
            Vector3 After = Generator.BezierTest(Generator.P1, Generator.P2, Generator.P3, Generator.P4, value_After); ;

            Handles.DrawLine(Before, After);
        }
    }
}
