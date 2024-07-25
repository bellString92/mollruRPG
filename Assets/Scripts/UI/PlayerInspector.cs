using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Player))]
public class PlayerInspector : Editor
{
    bool isVisble = true;
    public override void OnInspectorGUI()
    {
        if (isVisble = EditorGUILayout.Foldout(isVisble, "피카피카"))
        {
            base.OnInspectorGUI();
        }

    }
}
