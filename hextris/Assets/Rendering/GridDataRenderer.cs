using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridDataRenderer : MonoBehaviour {

    [SerializeField] GridData data;
    [SerializeField] HexRenderPool renderpool;

    readonly Dictionary<int, int> dID_ridx_lut = new();


    // ----------------------------------------------------------------------------------------------------------------
    // Data events

    private void OnEnable() => data.SubChange(AddRenderer, RemoveRenderer);
    private void OnDisable() => data.UnsubChange(AddRenderer, RemoveRenderer);

    private void AddRenderer(int data_id, SingleHexData data) {
        int new_render_idx = renderpool.InstantiateGFX(data);
        dID_ridx_lut.Add(data_id, new_render_idx);
    }

    private void RemoveRenderer(int data_id) {

        // Ask renderer to remove gfx & remove from internal listing so we don't continue to reference it
        int render_idx = dID_ridx_lut[data_id];
        renderpool.DestroyGFX(render_idx);
        dID_ridx_lut.Remove(data_id);

    }

}
