using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Hextris/Grid Data")]
public class GridData: SharedDataSO {

    private Dictionary<int, SingleHexData> hexes;
    private int _id_keeper;
    private IDGrid id_grid;


    // ----------------------------------------------------------------------------------------------------------------
    // Inherit from parent

    public override void Init() {
        this._id_keeper = 0;
        this.id_grid = new IDGrid();
        this.hexes = new();
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Events

    public Action<int, SingleHexData> HexAdded;
    public Action<int> HexRemoved;
    public void SubChange(Action<int, SingleHexData> OnAdd, Action<int> OnRemove) {
        HexAdded += OnAdd;
        HexRemoved += OnRemove;
    }

    public void UnsubChange(Action<int, SingleHexData> OnAdd, Action<int> OnRemove) {
        HexAdded -= OnAdd;
        HexRemoved -= OnRemove;
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Public - Data modifiers

    public void SetStartState(List<GridPosition> initial_positions) {

        // Bail if we get bad data
        if (initial_positions == null) {
            Debug.LogWarning("Grid Init failed: Missing initial positions! (null valued)");
        }

        foreach(GridPosition position in initial_positions) {
            SingleHexData new_data = new(position, Color.white);
            AddOne(new_data);
        }
    }

    public void Add(HexListSO hexes_to_add) {
        foreach((int _, SingleHexData new_data) in hexes_to_add.EnumeratedData()) {
            AddOne(new_data);
        }
    }

    public SingleHexData Remove(GridPosition position) {

        // Get ID lookup + data for return
        int cell_id = this.id_grid.GetID(position);
        SingleHexData hex_data = this.hexes[cell_id];

        // Remove data + record in id grid
        this.hexes.Remove(cell_id);
        this.id_grid.ClearCell(position);

        // Signal removal of hex data
        HexRemoved?.Invoke(cell_id);

        return hex_data;
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    private void AddOne(SingleHexData hex_data) {

        // Assign new (unique) id
        int new_id = this._id_keeper;
        this._id_keeper += 1;

        // Add data to both the hex listing & grid structure
        this.hexes.Add(new_id, hex_data);
        this.id_grid.SetID(hex_data.position, new_id);

        // Signal addition of new hex (by ID)
        HexAdded?.Invoke(new_id, hex_data);
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Public - Data checkers

    public bool IsOccupied(GridPosition position) => !this.id_grid.IsEmpty(position);

    public bool CanMoveTo(GridPosition position) {

        /* True if a cell could move into the target position (i.e. it's in-bounds & empty) */

        bool can_move_to = false;
        if (GridShape.IsInBounds(position)) {
            can_move_to = !IsOccupied(position);
        }

        return can_move_to;
    }


    public bool IsRowFull(int row_index) {

        /* True if all cells in a row are occupied */

        bool row_is_full = true;
        foreach (GridPosition position in GridShape.ColumnPositionIter(row_index)) {
            if (!IsOccupied(position)) {
                row_is_full = false;
                break;
            }
        }

        return row_is_full;
    }

    public bool IsRowEmpty(int row_index) {

        /* True if all cells in a row are empty */

        bool row_is_empty = true;
        foreach (GridPosition position in GridShape.ColumnPositionIter(row_index)) {
            if (IsOccupied(position)) {
                row_is_empty = false;
                break;
            }
        }

        return row_is_empty;
    }

    public IEnumerable<GridPosition> OccupiedInRowIter(int row_index) {

        /* Iterate over all occupied positions in a given row */

        foreach(GridPosition position in GridShape.ColumnPositionIter(row_index)) {
            if (IsOccupied(position)) {
                yield return position;
            }
        }
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Helper data type/accelerator

    private class IDGrid {

        const int EMPTY_ID = int.MinValue;
        readonly int[,] ids;


        // ----------------------------------------------------------------------------------------------------------------
        // Constructor(s)

        public IDGrid() {

            // Initialize id array as being empty
            ids = new int[GridShape.TOTAL_NUM_COLUMNS, GridShape.NUM_ROWS];
            foreach (GridPosition position in GridShape.AllPositionsIter()) {
                ClearCell(position);
            }

        }


        // ----------------------------------------------------------------------------------------------------------------
        // Cells

        public int GetID(GridPosition position) => this.ids[position.c, position.r];
        public void SetID(GridPosition position, int id) => this.ids[position.c, position.r] = id;
        public void ClearCell(GridPosition position) => SetID(position, EMPTY_ID);
        public bool IsEmpty(GridPosition position) => GetID(position) == EMPTY_ID;

    }

}