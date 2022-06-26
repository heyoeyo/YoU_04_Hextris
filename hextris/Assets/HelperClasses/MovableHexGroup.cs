using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableHexGroup {

    readonly HexGroup hex_group;
    readonly GridData grid;
    int next_drop_direction;
    HexListSO hexes;


    // ----------------------------------------------------------------------------------------------------------------
    // Constructor(s)

    public MovableHexGroup(ShapeSpec[] shape_specs, Color shape_color, GridData grid) {

        this.hex_group = new(shape_specs, shape_color);
        this.grid = grid;
        this.hexes = null;
    }

    public MovableHexGroup(MovableHexGroup other_group, Color shape_color) {
        this.hex_group = new(other_group.hex_group.shape_specs, shape_color);
        this.grid = other_group.grid;
        this.hexes = null;
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Public

    public SingleHexData[] CreateHexData() => this.hex_group.CreateHexData();

    public void MatchTo(MovableHexGroup other) {
        this.hex_group.MatchTo(other.hex_group);
        this.next_drop_direction = other.next_drop_direction;
    }

    public bool PlaceOnGrid(HexListSO hex_data) {

        // Store hex data for updating
        this.hexes = hex_data;

        // Figure out and initial guess at a reasonable starting position
        ShapeSpec curr_spec = GetSpec();
        int start_row = GridShape.NUM_ROWS - 1;
        GridPosition start_position = GridShape.GetMiddleColumn(start_row);

        // Try to place shape on grid to start
        (bool bumped_ok, GridPosition bumped_start_position) = CheckBumpedShapeOk(curr_spec, start_position);
        this.hex_group.position = bumped_ok ? bumped_start_position : start_position;

        // Set starting drop direction
        this.next_drop_direction = GridShape.IsEvenRow(this.hex_group.position) ? 1 : -1;

        // Update the hexes either way
        UpdateHexPositions();
        
        //Debug.Log("START: " + bumped_start_position);

        return bumped_ok;
    }

    public bool Move(int amount) {

        // Compute all new point positions after movement
        ShapeSpec curr_spec = GetSpec();
        GridPosition new_position = this.hex_group.position + new GridPosition(amount, 0);

        // Check if all new positions are open, if ok update all points
        bool move_is_ok = CheckShapeOk(curr_spec, new_position);
        if (move_is_ok) {
            this.hex_group.position = new_position;
            UpdateHexPositions();
        }

        // If move didn't work, adjust next drop to be in the same direction as attempted move
        // (can alter full-fall trajectory, even if the current move can't be made)
        if (!move_is_ok) next_drop_direction = Mathf.RoundToInt(Mathf.Sign(amount));

        return move_is_ok;
    }

    public bool Rotate(int amount) {

        // Figure out new point positions after rotation
        int new_rotation_idx = this.hex_group.GetUpdatedRotationIndex(amount);
        ShapeSpec rot_spec = this.hex_group.shape_specs[new_rotation_idx];

        // Check if all new positions are open
        (bool rot_is_ok, GridPosition bumped_position) = CheckBumpedShapeOk(rot_spec, this.hex_group.position);

        // If ok, update rotation setting
        if (rot_is_ok) {
            this.hex_group.rotation_idx = new_rotation_idx;
            this.hex_group.position = bumped_position;
            UpdateHexPositions();
        }

        return rot_is_ok;
    }

    public bool Drop() => Drop(true);
    public bool Drop(bool render_on_complete) {

        // For convenience
        bool drop_is_ok = false;
        ShapeSpec curr_spec = GetSpec();
        GridPosition new_position = this.hex_group.position;

        // Attempt drop twice (down-left/down-right) if needed, to look for open falling areas
        for (int k = 0; k < 2; k++) {
            new_position = this.hex_group.position + new Vector2Int(this.next_drop_direction, -1);
            drop_is_ok = CheckShapeOk(curr_spec, new_position);
            if (drop_is_ok) {
                break;
            }

            // Swap drop direction, so we'll try alternate direction on next attempt (as if we meant it all along)
            this.next_drop_direction *= -1;
        }

        // Toggle drop direction after each drop
        this.next_drop_direction *= -1;

        // If we got a valid drop, update all points
        if (drop_is_ok) {
            this.hex_group.position = new_position;
            if (render_on_complete) {
                UpdateHexPositions();
            }
        }

        return drop_is_ok;
    }

    public void DropFull() {

        // Drop till you stop
        bool drop_ok = true;
        while(drop_ok) {
            drop_ok = Drop(false);
        }

        // Only re-position hexes after we've fully dropped
        UpdateHexPositions();

    }

    public IEnumerable<GridPosition> PositionIter() => this.hex_group.PositionIter();


    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    private ShapeSpec GetSpec() => this.hex_group.GetSpec();

    private bool CheckShapeOk(ShapeSpec shape_spec, GridPosition position) {

        // Check that all shape positions are valid, with the given positioning
        bool shape_ok = true;
        foreach (GridPosition hex_pos in shape_spec.PositionIter(position)) {
            if (!this.grid.CanMoveTo(hex_pos)) {
                shape_ok = false;
                break;
            }
        }

        return shape_ok;
    }

    private (bool, GridPosition) CheckBumpedShapeOk(ShapeSpec shape_spec, GridPosition position) {

        // Bail if original shape is already ok
        GridPosition bumped_position = position;
        bool bumped_shape_ok = CheckShapeOk(shape_spec, bumped_position);
        if (bumped_shape_ok) {
            return (bumped_shape_ok, bumped_position);
        }

        // Check if the shape fits with any of the position bumps
        Vector2Int[] bump_offsets = shape_spec.GetBumpOffsets();
        foreach (Vector2Int bump in bump_offsets) {
            bumped_position = position + bump;
            bumped_shape_ok = CheckShapeOk(shape_spec, bumped_position);
            if (bumped_shape_ok) {
                break;
            }
        }

        return (bumped_shape_ok, bumped_position);
    }

    private void UpdateHexPositions() {
        SingleHexData[] new_hex_data = CreateHexData();
        this.hexes.Replace(new_hex_data);
    }
}