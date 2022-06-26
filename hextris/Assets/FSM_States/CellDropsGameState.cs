using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCellsGameState : GameState {

    readonly GSManager parent;

    public DropCellsGameState(GSManager parent) {
        this.parent = parent;
    }


    // ----------------------------------------------------------------------------------------------------------------
    // State functions

    public override void Enter() {

        // Make sure we have cleared rows to drop into, otherwise we're done with this state
        bool no_rows_to_drop = parent.cleared_row_idxs.Count == 0;
        if (no_rows_to_drop) {
            LeaveState();
            return;
        }

        // Grab top-most cleared row index, since we need to drop everything above it
        int cleared_row = parent.cleared_row_idxs.Pop();
        int first_row_to_drop = 1 + cleared_row;
        int x_shift = GridShape.IsEvenRow(first_row_to_drop) ? 1 : -1;
        GridPosition offset_to_drop = new(x_shift, -1);
        GridPosition opposite_offset_to_drop = new(-x_shift, -1);
        bool is_left_shift = (x_shift < 0);

        // Grab all cells above missing row and check if they can be dropped
        List<SingleHexData> hexes_to_drop_left = new();
        List<SingleHexData> hexes_to_drop_right = new();
        for (int row_idx = first_row_to_drop; row_idx < GridShape.NUM_ROWS; row_idx++) {

            // Dropping may leave a blank (unreachable spot) on far edge of row due to zig-zap pattern of the grid,
            // so perform an extra check to see if we should drop a duplicate hex in the opposite direction
            bool check_corner_drop = !(is_left_shift ^ GridShape.IsEvenRow(row_idx));
            GridPosition corner_position = is_left_shift ? GridShape.GetLastColumn(row_idx) : GridShape.GetFirstColumn(row_idx);

            foreach (GridPosition position in parent.grid.OccupiedInRowIter(row_idx)) {
                GridPosition drop_position = position + offset_to_drop;
                if (parent.grid.CanMoveTo(drop_position)) {

                    // Remove cell from main grid and into animating group (left/right drop as needed)
                    SingleHexData hex_data = parent.grid.Remove(position);
                    if (is_left_shift) hexes_to_drop_left.Add(hex_data);
                    else hexes_to_drop_right.Add(hex_data);

                    // Check for special case of corner gaps
                    // TODO: clean this up, it's really messy...
                    if (check_corner_drop) {

                        // If we're dealing with a corner position can it has space to drop, then
                        // create a duplicate hex that drops both left & right, to fill would-be gap!
                        bool is_corner_position = (position.c == corner_position.c);
                        if (is_corner_position) {
                            GridPosition opposite_drop_position = position + opposite_offset_to_drop;
                            if (parent.grid.CanMoveTo(opposite_drop_position)) {
                                SingleHexData copy_hex = new(hex_data.position, hex_data.color);
                                if (is_left_shift) hexes_to_drop_right.Add(copy_hex);
                                else hexes_to_drop_left.Add(copy_hex);
                            }

                        }
                    }
                }
            }

        }

        // Check if we ended up with actual drop data!
        // (it's possible, due to wall-jamming, that no hexes ended up dropping)
        bool have_drop_data = (hexes_to_drop_left.Count > 0 || hexes_to_drop_right.Count > 0);
        if (!have_drop_data) {
            LeaveState();
            return;
        }

        // Move data into drop animators
        parent.dropped_left_hexes.Add(hexes_to_drop_left.ToArray());
        parent.dropped_left_hexes.BeginAnimating();
        parent.dropped_right_hexes.Add(hexes_to_drop_right.ToArray());
        parent.dropped_right_hexes.BeginAnimating();
    }

    public override void Update() {

        // Once animation completes, repeat drop state to check for other empty rows
        bool still_animating = parent.dropped_left_hexes.is_animating || parent.dropped_right_hexes.is_animating;
        if (!still_animating) {
            //Debug.LogWarning("REPEATING CELL DROP STATE");
            parent.EnterDropCells();
            return;
        }
    }

    public override void Exit() {

        // Add hexes back to grid then wipe out reference data to clean up rendering
        parent.grid.Add(parent.dropped_left_hexes);
        parent.grid.Add(parent.dropped_right_hexes);
        parent.dropped_left_hexes.Clear();
        parent.dropped_right_hexes.Clear();
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    void LeaveState() => parent.EnterSpawn();

}
