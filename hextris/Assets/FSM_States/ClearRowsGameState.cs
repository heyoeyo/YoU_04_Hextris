using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearRowsGameState : GameState
{
    readonly GSManager parent;


    // ----------------------------------------------------------------------------------------------------------------
    // Constructor(s)

    public ClearRowsGameState(GSManager parent) {
        this.parent = parent;
    }


    // ----------------------------------------------------------------------------------------------------------------
    // State functions

    public override void Enter() {

        // If no rows are full, move to spawn state
        parent.cleared_row_idxs = FindRowsToClear();
        bool no_rows_to_clear = (parent.cleared_row_idxs.Count == 0);
        if (no_rows_to_clear) {
            //Debug.Log("NO ROWS TO CLEAR!");
            parent.EnterSpawn();
            return;
        }

        // Get reference to all hexes that need to be cleared for animating
        List<SingleHexData> hexes_to_remove = new();
        foreach(int row_idx in parent.cleared_row_idxs) {
            foreach(GridPosition position in GridShape.ColumnPositionIter(row_idx)) {
                SingleHexData hex_data = parent.grid.Remove(position);
                hexes_to_remove.Add(hex_data);
            }
            EventsManager.TriggerClearRow();
        }

        // Move hex data over to removal group, for removal animation rendering
        parent.removal_hexes.Add(hexes_to_remove.ToArray());
        parent.removal_hexes.BeginAnimating();
    }


    public override void Update() {

        // Move to drop state once animations end
        if (!parent.removal_hexes.is_animating) {
            parent.EnterDropCells();
            return;
        }
    }

    public override void Exit() {

        // Wipe out removal hexes, so rendering cleans up
        parent.removal_hexes.Clear();
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    private Stack<int> FindRowsToClear() {

        /*
        Records which rows are full, so need to be cleared
        Order is bottom-to-top so that popping stack gives top-to-bottom ordering!
        */

        // Record indexes of rows that are full (i.e. need to be cleared)
        Stack<int> rows_to_clear = new();
        foreach (int row_idx in GridShape.RowIndexIterBottomUp()) {
            if (parent.grid.IsRowFull(row_idx)) {
                rows_to_clear.Push(row_idx);
            }
        }

        return rows_to_clear;

    }
}
