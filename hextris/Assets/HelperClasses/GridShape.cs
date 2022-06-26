using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class GridShape {

    readonly public static int NUM_ROWS = 20;
    readonly public static int NUM_COLUMNS_PER_ROW = 10;
    readonly public static int TOTAL_NUM_COLUMNS = NUM_COLUMNS_PER_ROW * 2;


    // ----------------------------------------------------------------------------------------------------------------
    // Checkers

    public static bool IsEvenRow(int row_index) => (row_index % 2) == 0;
    public static bool IsEvenRow(GridPosition position) => IsEvenRow(position.r);

    public static bool IsInBounds(GridPosition position) {

        /* Checks whether a given colrow position is even valid on the grid */

        // Check we're indexing a valid point in the grid array
        bool lr_inbounds = (position.c >= 0 && position.c < TOTAL_NUM_COLUMNS);
        bool ud_inbounds = (position.r >= 0 && position.r < NUM_ROWS);

        // Check the index lands on a valid hex cell!
        bool same_parity = ((position.c + position.r) % 2) == 0;

        return (lr_inbounds && ud_inbounds && same_parity);
    }

    public static GridPosition GetMiddleColumn(int row_index) {

        // Get all valid column indexes for the given row and find center index
        List<int> all_col_idxs = ColumnIndexIter(row_index).ToList();
        float middle_list_idx = (float)(all_col_idxs.Count - 1.0f) / 2.0f;
        int upper_idx = Mathf.CeilToInt(middle_list_idx);
        //int lower_idx = Mathf.FloorToInt(middle_list_idx);

        // Upper index seems to be a better fit for starting point?
        int selected_col_idx = all_col_idxs[upper_idx];

        return new GridPosition(selected_col_idx, row_index);
    }

    public static GridPosition GetFirstColumn(int row_index) {
        int first_col_idx = GetValidColumnIndex(0, IsEvenRow(row_index));
        return new GridPosition(first_col_idx, row_index);
    }

    public static GridPosition GetLastColumn(int row_index) {
        int last_col_idx = GetValidColumnIndex(NUM_COLUMNS_PER_ROW - 1, IsEvenRow(row_index));
        return new GridPosition(last_col_idx, row_index);
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Iterators

    public static IEnumerable<GridPosition> AllPositionsIter() {

        foreach (int row_idx in RowIndexIterBottomUp()) {
            foreach (GridPosition position in ColumnPositionIter(row_idx)) {
                yield return position;
            }
        }
    }

    public static IEnumerable<int> RowIndexIterBottomUp() {

        /* Simple iterator over all row indices (bottom-to-top) */

        for (int row_idx = 0; row_idx < NUM_ROWS; row_idx++) {
            yield return row_idx;
        }
    }

    public static IEnumerable<int> RowIndexIterTopDown() {
        foreach(int botup_row_idx in RowIndexIterBottomUp()) {
            yield return NUM_ROWS - 1 - botup_row_idx;
        }
    }

    public static IEnumerable<GridPosition> ColumnPositionIter(int row_index) {

        foreach (int col_idx in ColumnIndexIter(row_index)) {
            yield return new GridPosition(col_idx, row_index);
        }
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Internal helpers

    private static IEnumerable<int> ColumnIndexIter(int row_index) {

        /* Iterator over (within-grid) column indices for a given row */

        bool is_even_row = IsEvenRow(row_index);
        for (int col_idx_in_row = 0; col_idx_in_row < NUM_COLUMNS_PER_ROW; col_idx_in_row++) {
            yield return GetValidColumnIndex(col_idx_in_row, is_even_row);
        }
    }

    private static int GetValidColumnIndex(int col_index_within_row, bool is_even_row) {
        /*
        Converts column indexing within a row (i.e. zeroth, first, second etc. column)
        into grid-wide column indexing which accounts for even/odd row offsets,
        as well as the 2x multiplier within a row (i.e. adjacent columns are 2 units apart!)
        */
        return (2 * col_index_within_row) + (is_even_row ? 0 : 1);
    }
}
