using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeSO))]
public class ShapeEditor : Editor {

    const int num_half_rows = 2;
    const int num_half_cols = 2;
    List<Vector2Int> points_in_shape;

    // For convenience
    ShapeSO targ;
    Color target_color;

    // Set up central point used for orienting shapes
    readonly Vector2Int pivot_point = Vector2Int.zero;

    // Set up draw sizing
    readonly static float box_size = EditorGUIUtility.singleLineHeight * 2;
    readonly GUILayoutOption btn_w = GUILayout.Width(box_size);
    readonly GUILayoutOption btn_h = GUILayout.Height(box_size);

    private void OnEnable() {
        targ = (ShapeSO)target;
        points_in_shape = new();
    }

    public override void OnInspectorGUI() {

        // Show original UI elements
        base.OnInspectorGUI();

        // Set up coloring, which behaves strangely
        target_color = targ.color + new Color(0, 0, 0, 1);

        // Make sure we have a drawing data set before doing anything else
        if (points_in_shape == null) {
            points_in_shape = new();
        }

        // Leave space for readability
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        DrawRerunConstructorButton();

        // Leave space for readability
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        DrawOrientations();

        // Leave space for readability
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        DrawShapeInputter();

        // Leave space for readability
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        DrawAddOrientationButton();

    }


    // ---------------------------------------------------------------------------------------------------------------
    // Main UI components

    void DrawOrientations() {

        // Get number of current orientations, with some error handling for race conditions on startup?
        int num_orientations = 0;
        if (targ.shape_specs != null) {
            num_orientations = targ.shape_specs.Count;
        }

        // Set sizing for individual shape points (in each orientation preview)
        const int point_size = 4;
        const int point_spacing = 1;
        Vector2 point_wh = new(point_size, point_size);

        // Set larger cell sizing (i.e. box holding each shape)
        Rect last_rect = GUILayoutUtility.GetLastRect();
        const int cell_size = 50;
        const int cell_spacing = 5;
        
        // Figure out how much space we have in x, to help center GFX
        float total_cell_width = (cell_size + cell_spacing) * num_orientations - cell_spacing;
        float max_width = EditorGUIUtility.currentViewWidth;
        float x_offset = (max_width - total_cell_width) / 2f;


        Vector2 cell_wh = new(cell_size, cell_size);
        Vector2 origin_xy = new(x_offset, last_rect.y);
        for (int k = 0; k < num_orientations; k++) {

            // Draw bordered rectangle to contain each orientation
            Vector2 cell_pos = k * (cell_size + cell_spacing) * Vector2.right + origin_xy;
            Rect border_rect = new(cell_pos, cell_wh);
            Rect cell_rect = new(cell_pos + 2 * Vector2.one, cell_wh - 4 * Vector2.one);
            EditorGUI.DrawRect(border_rect, Color.black);
            EditorGUI.DrawRect(cell_rect, new Color(0.2f, 0.2f, 0.2f));

            // Draw little shape points to indicate shape/orientation
            GridPosition zero_pos = new(0, 0);
            Vector2 point_origin = cell_pos + (cell_wh - point_wh) * 0.5f;
            Rect point_rect = new(point_origin, point_wh);
            foreach(GridPosition pt in targ.shape_specs[k].PositionIter(zero_pos)) {
                Color pt_color = pt.IsZero() ? Color.white : target_color;
                point_rect.position = point_origin + new Vector2(0.5f * pt.c, -1*pt.r) * (point_size + point_spacing);
                EditorGUI.DrawRect(point_rect, pt_color);
            }
        }

        // Figure out bottom-most point and add spacing to force everything beneath to move
        EditorGUILayout.Space(last_rect.y + cell_size);
    }

    void DrawShapeInputter() {

        /* Function used to draw hex-style staggered grid for defining shape points */

        EditorGUILayout.BeginVertical();
        for (int r = num_half_rows; r >= -num_half_rows; r--) {

            // Set up proper x-indent
            bool is_even_row = (r % 2) == 0;
            float row_x_offset = is_even_row ? 0 : box_size;
            int c_idx_offset = is_even_row ? 0 : 1;

            // Get proper staggered row spacing
            GUILayout.Space(box_size / 12f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Space(row_x_offset);

            for (int c = -num_half_cols; c <= num_half_cols; c++) {

                // Convert row-specific column index to 'hex' index
                int hex_c = c_idx_offset + 2 * c;
                Vector2Int target_point = new(hex_c, r);

                // Handle special case of center pivot point
                if (target_point == pivot_point) {
                    CreatePivotGFX(btn_w, btn_h);
                    continue;
                }

                // Draw toggle-able inputs if we're not looking at the pivot point
                TogglePointBtn(target_point, btn_w, btn_h);
            }

            // End row spacing
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndVertical();
    }

    void DrawAddOrientationButton() {

        // Don't allow the user to add orientations if there aren't enough points
        bool has_enough_points = (points_in_shape.Count == 3);
        string add_btn_msg = has_enough_points ? "Add new orientation" : "Need more points";
        GUI.enabled = has_enough_points;
        bool add_new_shape = GUILayout.Button(add_btn_msg);
        if (add_new_shape) {
            ShapeSpec new_spec = new(points_in_shape.ToArray());
            targ.AddOrientation(new_spec);
        }
        GUI.enabled = true;
    }

    void DrawRerunConstructorButton() {

        /* Re-runs the constructor of each shape spec and replaces them in the target (used in case spec constructor changes) */

        bool rerun_constructors = GUILayout.Button("Re-run constructors");
        if (rerun_constructors) {
            ShapeSpec[] orig_specs = targ.shape_specs.ToArray();
            targ.shape_specs = null;
            foreach(ShapeSpec spec in orig_specs) {
                ShapeSpec new_spec = new(spec.offsets);
                targ.AddOrientation(new_spec);
            }
        }
    }


    // ---------------------------------------------------------------------------------------------------------------
    // Helpers

    void CreatePivotGFX(GUILayoutOption button_width, GUILayoutOption button_height) {

        /* Draw a special, non-interactable button to represent center/pivot point of shape */

        Color orig_color = GUI.color;
        GUI.color = 2 * Color.white + 3 * target_color;
        GUI.enabled = false;
        GUILayout.Button("", button_width, button_height);
        GUI.enabled = true;
        GUI.color = orig_color;
    }

    void TogglePointBtn(Vector2Int target_point, GUILayoutOption button_width, GUILayoutOption button_height) {

        // Store original color so we can restore it after modification
        Color orig_color = GUI.color;

        // Set coloring of target point, based on whether it is currently active
        bool is_active = points_in_shape.Contains(target_point);
        if (is_active) {
            GUI.color = 3 * target_color;
        }

        // Update list of points if the toggle is clicked
        bool clicked = GUILayout.Button("", button_width, button_height);
        if (clicked) {
            UpdateDrawnPoints(target_point);
        }

        // Reset coloring
        GUI.color = orig_color;
    }

    void UpdateDrawnPoints(Vector2Int input_point) {

        /* Function used to update currently drawn shape points */

        // Check if we're adding/removing points
        bool in_list = points_in_shape.Contains(input_point);
        if (in_list) {
            points_in_shape.Remove(input_point);
        } else {
            points_in_shape.Add(input_point);
        }

        // Remove oldest point(s) if we end up with too many points
        while (points_in_shape.Count > 3) {
            points_in_shape.RemoveAt(0);
        }
    }
}
