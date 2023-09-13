using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SkinnedCollisionHelper))]
public class SkinnedCollisionHelper_Inspector : Editor
{
    SkinnedCollisionHelper script;

    private void OnEnable()
    {
        script = target as SkinnedCollisionHelper;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Update Collision Mesh"))
        {
            script.UpdateCollisionMesh();
        }
    }
}
