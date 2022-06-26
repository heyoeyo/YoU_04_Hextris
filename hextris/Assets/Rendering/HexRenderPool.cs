using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexRenderPool : MonoBehaviour
{
    [SerializeField] HexGFX hex_gfx_prefab;
    [SerializeField] TileRenderer tile_renderer;

    HexGFX[] hex_gfx_pool;
    readonly Queue<int> idx_unused = new();


    // ----------------------------------------------------------------------------------------------------------------
    // Built-ins

    void Awake() => FillPoolIfNeeded();

    private void OnEnable() => FillPoolIfNeeded();

    private void OnDisable() {
        // Wipe out render data on disabling
        int num_hexes = hex_gfx_pool.Length;
        for (int i = 0; i < num_hexes; i++) {
            Destroy(this.hex_gfx_pool[i]);
        }
        this.hex_gfx_pool = null;
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Public

    public int InstantiateGFX(SingleHexData hex_data) {

        // Grab an unused idx, activate the object & set it's position/color
        int new_idx = this.idx_unused.Dequeue();
        HexGFX gfx_ref = this.hex_gfx_pool[new_idx];
        gfx_ref.gameObject.SetActive(true);

        // Initialize according to given inputs
        Vector2 render_position = this.tile_renderer.GetRenderXY(hex_data.position);
        gfx_ref.SetBaseScale(tile_renderer.GetGridScale());
        gfx_ref.SetColor(hex_data.color);
        gfx_ref.SetPosition(render_position);

        return new_idx;
    }
    public void DestroyGFX(int index) {
        // Disable target gfx & flag index as unused again (don't actually destroy the game object!)
        this.hex_gfx_pool[index].gameObject.SetActive(false);
        this.idx_unused.Enqueue(index);
    }

    public void SetGFXPosition(int index, Vector2 grid_position) {
        Vector2 render_position = this.tile_renderer.GetRenderXY(grid_position);
        this.hex_gfx_pool[index].SetPosition(render_position);
    }
    public void SetGFXPosition(int index, GridPosition position) {
        Vector2 render_position = this.tile_renderer.GetRenderXY(position);
        this.hex_gfx_pool[index].SetPosition(render_position);
    }

    public void SetGFXRotation(int index, float rotaion_deg) => this.hex_gfx_pool[index].SetRotation(rotaion_deg);
    public void SetGFXScale(int index, float scale) => this.hex_gfx_pool[index].SetScale(scale);
    public void SetGFXColor(int index, Color color) => this.hex_gfx_pool[index].SetColor(color);



    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    void FillPoolIfNeeded() {

        // Only fill pool if it is empty/doesn't exist
        bool pool_missing = (this.hex_gfx_pool == null) || (this.hex_gfx_pool.Length == 0) ;
        if(!pool_missing) {
            return;
        }

        // We'll create enough hexes to fill all possible grid-cells + a bit extra just in case
        // (practically this is major overkill... but simple)
        int num_grid_cells = (GridShape.NUM_COLUMNS_PER_ROW * GridShape.NUM_ROWS);
        int num_hexes = num_grid_cells + 12;

        // Create all the gfx copies we need, make sure they're disabled and tag all indexes as unused
        this.hex_gfx_pool = new HexGFX[num_hexes];
        for (int i = 0; i < num_hexes; i++) {
            HexGFX new_hex = Instantiate(hex_gfx_prefab, this.transform);
            new_hex.gameObject.SetActive(false);
            this.hex_gfx_pool[i] = new_hex;
            this.idx_unused.Enqueue(i);
        }
    }
}