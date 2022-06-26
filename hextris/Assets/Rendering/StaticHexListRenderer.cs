using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticHexListRenderer : MonoBehaviour {

    [SerializeField] HexListSO data;
    [SerializeField] TileRenderer tile_renderer;
    [SerializeField] HexGFX hex_gfx_prefab;
    [SerializeField] Vector2 render_offset;

    readonly List<HexGFX> renderers = new();

    readonly Queue<int> unused_idxs = new();
    readonly List<HexGFX> renderpool = new();


    // ----------------------------------------------------------------------------------------------------------------
    // Events

    private void OnEnable() => data.SubChange(AddRenderer, ClearRenderers);
    private void OnDisable() => data.UnsubChange(AddRenderer, ClearRenderers);

    void AddRenderer(SingleHexData hex_data) {

        HexGFX new_gfx = InstantiateFromPool();

        // Create new gfx prefab with appropriate size, color & position
        Vector2 render_position = tile_renderer.GetRenderXY(hex_data.position);
        //HexGFX new_gfx = Instantiate(hex_gfx_prefab, this.transform);
        new_gfx.SetBaseScale(tile_renderer.GetGridScale());
        new_gfx.SetColor(hex_data.color);
        new_gfx.SetPosition(render_position + render_offset);

        // Add to renderer listing
        renderers.Add(new_gfx);
    }

    void ClearRenderers() {

        unused_idxs.Clear();
        int idx = 0;
        foreach(HexGFX hex_gfx in renderpool) {
            hex_gfx.gameObject.SetActive(false);
            unused_idxs.Enqueue(idx);
            idx += 1;
        }


        /*
        // Wipe out all rendered prefabs
        foreach (HexGFX gfx_ref in this.renderers) {
            Destroy(gfx_ref.gameObject);
        }

        // Wipe out data references
        this.renderers.Clear();
        */
    }

    void AddToPool() {

        // Create new game object for rendering
        HexGFX new_gfx = Instantiate(hex_gfx_prefab, this.transform);
        new_gfx.gameObject.SetActive(false);
        renderpool.Add(new_gfx);

        // Keep track of the index of the newly added game object for re-use
        int new_idx = renderpool.Count - 1;
        unused_idxs.Enqueue(new_idx);
    }

    HexGFX InstantiateFromPool() {

        // If no renderers are available, add a new entry to deal with request
        bool no_renderers_available = (unused_idxs.Count == 0);
        if (no_renderers_available) {
            AddToPool();
        }

        // Find an unused pool index
        int pool_idx = unused_idxs.Dequeue();

        // Grab the renderer and make sure it's active (as-if it was just instantiated)
        HexGFX new_gfx = renderpool[pool_idx];
        new_gfx.gameObject.SetActive(true);

        return new_gfx;
    }

}
