using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QueueSO))]
public class QueueEditor : Editor {

    public override void OnInspectorGUI() {

        // Show original UI elements
        base.OnInspectorGUI();

        // Show queue listing
        EditorGUILayout.LabelField("PREVIEW", EditorStyles.centeredGreyMiniLabel);
        
        // Draw queue names in comma-separated row
        EditorGUILayout.BeginHorizontal();
        QueueSO queue = target as QueueSO;
        string all_names= string.Join(", ", queue.GetNames());
        EditorGUILayout.LabelField(all_names);
        EditorGUILayout.EndHorizontal();
    }

}
