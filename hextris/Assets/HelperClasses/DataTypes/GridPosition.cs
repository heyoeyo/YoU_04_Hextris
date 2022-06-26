using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GridPosition {

    public int c;
    public int r;

    public GridPosition(int column_index, int row_index) {
        this.c = column_index;
        this.r = row_index;
    }

    public bool IsZero() => (this.c == 0) && (this.r == 0);

    public static Vector2 Lerp(GridPosition start, GridPosition end, float t) {
        return new Vector2(Mathf.Lerp(start.c, end.c, t), Mathf.Lerp(start.r, end.r, t));
    }

    public static GridPosition operator +(GridPosition pos_1, GridPosition pos_2) {
        return new GridPosition(pos_1.c + pos_2.c, pos_1.r + pos_2.r);
    }

    public static GridPosition operator +(GridPosition position, Vector2Int offset) {
        return new GridPosition(position.c + offset.x, position.r + offset.y);
    }
    public static GridPosition operator +(Vector2Int offset, GridPosition position) => position + offset;

    public static GridPosition operator -(GridPosition pos_1, GridPosition pos_2) {
        return new GridPosition(pos_1.c - pos_2.c, pos_1.r - pos_2.r);
    }

    public static GridPosition operator -(GridPosition position, Vector2Int offset) {
        return new GridPosition(position.c - offset.x, position.r - offset.y);
    }

    public override string ToString() => string.Format("C:{0}, R:{1}", this.c, this.r);
}
