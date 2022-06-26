using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexListSO))]
public class HexListSOEditor : Editor {

    // Set up draw sizing
    readonly static float box_size = EditorGUIUtility.singleLineHeight;
    readonly GUILayoutOption btn_w = GUILayout.Width(box_size);
    readonly GUILayoutOption btn_h = GUILayout.Height(box_size);

    public override void OnInspectorGUI() {

        HexListSO targ = (HexListSO)target;
        if (targ.init_mask == null) {
            targ.init_mask = new();
        }

        // Show original UI elements
        base.OnInspectorGUI();

        EditorGUILayout.BeginVertical();
        foreach (int row in GridShape.RowIndexIterTopDown()) {

            // Set up proper x-indent to give zig-zag pattern to rows
            bool is_even_row = (row % 2) == 0;
            float row_x_offset = is_even_row ? 0 : box_size;

            // Get proper staggered row spacing
            GUILayout.Space(box_size / 12f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Space(row_x_offset);

            foreach (GridPosition position in GridShape.ColumnPositionIter(row)) {
                TogglePointBtn(position, btn_w, btn_h);
            }

            // End row spacing
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();


        // DRAW CLEARING BUTTON
        bool clear_data = GUILayout.Button("Clear Data");
        if (clear_data) {
            targ.init_mask = new();
        }

    }

    void TogglePointBtn(GridPosition position, GUILayoutOption button_width, GUILayoutOption button_height) {

        // For convenience
        HexListSO targ = (HexListSO)target;

        // Store original color so we can restore it after modification
        Color orig_color = GUI.color;

        // Change color based on whether it is included in data or not
        bool is_in_data = targ.init_mask.Contains(position);
        if (is_in_data) {
            GUI.color = new Color(10, 10, 10, 1);
        }

        // Update list of points if the toggle is clicked
        bool clicked = GUILayout.Button("", button_width, button_height);
        if (clicked) {
            if (is_in_data) {
                targ.init_mask.Remove(position);
            } else {
                targ.init_mask.Add(position);
            }
        }

        // Reset coloring
        GUI.color = orig_color;
    }

}