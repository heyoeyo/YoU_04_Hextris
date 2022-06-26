using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hextris/Tile Renderer")]
public class TileRenderer : ScriptableObject {

    [SerializeField] float scale = 0.5f;

    // For clarity, since we need this often (equivalent to cos(30deg) or sin(60deg))
    readonly float sqrt_3_by_2 = Mathf.Sqrt(3f) / 2f;


    // ----------------------------------------------------------------------------------------------------------------
    // Public

    public float GetGridScale() => this.scale;

    public Vector2 GetRenderXY(Vector2 position) => GetRenderXY(position.x, position.y);
    public Vector2 GetRenderXY(GridPosition position) => GetRenderXY(position.c, position.r);
    public Vector2 GetRenderXY(float col_index, float row_index) {

        /* 
        Function which maps row/column indices to physical x/y points
        Assumes bottom left hex is (0, 0), and column index increases
        by 2 while traveling left-to-right along a given row
        */

        // Map cell indices to physical locations
        Vector2 offsets = GetGridOffsets();
        float x = col_index * GetColStep() + offsets.x;
        float y = row_index * GetRowStep() + offsets.y;

        return new Vector2(x, y);
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Internal derived properties

    Vector2 GetGridOffsets() {

        // Calculate how much to shift the grid so that it is horizontally centered and 'standing up' from (0, 0) point
        float x_centering = (0.5f - GridShape.NUM_COLUMNS_PER_ROW) * (GetHexWidth() / 2f);
        float y_offset = GetHexHeight() / 2f;

        return new Vector2(x_centering, y_offset);
    }

    float GetHexRadius() => this.scale;

    float GetHexInnerRadius() => GetHexRadius() * sqrt_3_by_2;

    float GetHexWidth() => 2f * GetHexInnerRadius();

    float GetHexHeight() => 2f * GetHexRadius();

    float GetColStep() => sqrt_3_by_2 * GetHexRadius();

    float GetRowStep() => (3f / 2f) * GetHexRadius();

}
