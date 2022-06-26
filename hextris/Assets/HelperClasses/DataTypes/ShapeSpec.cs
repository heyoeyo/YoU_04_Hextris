using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShapeSpec {

    public Vector2Int[] offsets;
    int min_x, min_y, max_x, max_y;

    // ----------------------------------------------------------------------------------------------------------------
    // Constructors

    public ShapeSpec(Vector2Int[] offsets) {

        // Sanity check
        if (offsets.Length != 3) {
            Debug.LogError("Creating shapes without 3 unique offsets is not allowed!");
        }

        // Store provided offsets
        this.offsets = new Vector2Int[3];
        for (int i = 0; i < this.offsets.Length; i++) {
            this.offsets[i] = offsets[i];
        }

    }


    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    public int GetCount() => 1 + this.offsets.Length;

    public Vector2Int[] GetBumpOffsets() {

        // Find shape extents if needed
        bool missing_extents = (Mathf.Abs(min_x) + Mathf.Abs(max_x) + Mathf.Abs(min_y) + Mathf.Abs(max_x)) == 0;
        if (missing_extents) {
            FindExtents();
        }

        // Gather all offsets
        List<Vector2Int> bump_offsets = new();
        for (int k = 1; k <= Mathf.Abs(min_x); k++) bump_offsets.Add(Vector2Int.left * k);
        for (int k = 1; k <= Mathf.Abs(max_x); k++) bump_offsets.Add(Vector2Int.right * k);

        // Handle zig-zagging downward checks
        for (int k = 1; k <= Mathf.Abs(max_y); k++) {
            bool is_even_iter = (k % 2) == 0;
            if (is_even_iter) {
                bump_offsets.Add(Vector2Int.down * k);
            } else {
                Vector2Int down_left = new Vector2Int(-1, -k);
                Vector2Int down_right = new Vector2Int(1, -k);
                bump_offsets.Add(down_left);
                bump_offsets.Add(down_right);
            }
        }

        // Handle zig-zagging upward checks
        for (int k = 1; k <= Mathf.Abs(min_y); k++) {
            bool is_even_iter = (k % 2) == 0;
            if (is_even_iter) {
                bump_offsets.Add(Vector2Int.up * k);
            } else {
                Vector2Int up_left = new Vector2Int(-1, k);
                Vector2Int up_right = new Vector2Int(1, k);
                bump_offsets.Add(up_left);
                bump_offsets.Add(up_right);
            }
        }

        return bump_offsets.ToArray();
    }

    public IEnumerable<GridPosition> PositionIter(GridPosition pivot_position) {

        /* Iterator over all shape positions (offset by provided pivot) */

        // Return origin/pivot and then all other points
        yield return pivot_position;
        foreach (Vector2Int offset in offsets) {
            yield return pivot_position + offset;
        }

    }

    private void FindExtents() {

        // Find x/y extents
        this.min_x = this.min_y = 0;
        this.max_x = this.max_y = 0;
        foreach (Vector2Int offset in this.offsets) {
            this.min_x = Mathf.Min(this.min_x, offset.x);
            this.max_x = Mathf.Max(this.max_x, offset.x);
            this.min_y = Mathf.Min(this.min_y, offset.y);
            this.max_y = Mathf.Max(this.max_y, offset.y);
        }

    }
}
