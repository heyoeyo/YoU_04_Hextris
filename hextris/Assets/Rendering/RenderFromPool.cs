using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderFromPool {

    public HexRenderPool pool;
    public List<int> idxs;

    public RenderFromPool(HexRenderPool renderpool) {
        this.pool = renderpool;
        this.idxs = new();
    }

    public void AddRenderer(SingleHexData hex_data) {
        int new_render_idx = pool.InstantiateGFX(hex_data);
        this.idxs.Add(new_render_idx);
    }

    public void ClearRenderers() {
        foreach (int idx in this.idxs) {
            pool.DestroyGFX(idx);
        }
        idxs.Clear();
    }
}
