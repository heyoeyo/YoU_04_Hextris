using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGroup {

    public int rotation_idx = 0;
    public ShapeSpec[] shape_specs;
    public GridPosition position;
    readonly Color shape_color;


    // ----------------------------------------------------------------------------------------------------------------
    // Constructors

    public HexGroup(ShapeSpec[] shape_specs, Color shape_color, GridPosition position) {
        this.shape_specs = shape_specs;
        this.shape_color = shape_color;
        this.position = position;
    }
    public HexGroup(ShapeSpec[] shape_specs, Color shape_color) : this(shape_specs, shape_color, new GridPosition(0, 0)) { }


    // ----------------------------------------------------------------------------------------------------------------
    // Public

    public void MatchTo(HexGroup other) {
        this.position = other.position;
        this.rotation_idx = other.rotation_idx;
    }

    public ShapeSpec GetSpec() => this.shape_specs[this.rotation_idx];

    public SingleHexData[] CreateHexData() {

        ShapeSpec curr_spec = GetSpec();

        List<SingleHexData> hex_data = new();
        foreach(GridPosition hex_position in curr_spec.PositionIter(this.position)) {
            hex_data.Add(new SingleHexData(hex_position, this.shape_color));
        }

        return hex_data.ToArray();
    }

    public IEnumerable<GridPosition> PositionIter() {
        ShapeSpec curr_spec = GetSpec();
        foreach (GridPosition pos in curr_spec.PositionIter(this.position)) {
            yield return pos;
        }
    }

    public int GetUpdatedRotationIndex(int amount) {

        int new_rot_idx = (this.rotation_idx + amount) % this.shape_specs.Length;
        if (new_rot_idx < 0) {
            new_rot_idx += this.shape_specs.Length;
        }

        return new_rot_idx;
    }

}
