using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolRenderer : MonoBehaviour {

    [SerializeField] HexListSO data;
    [SerializeField] HexRenderPool renderpool;

    public RenderFromPool rfp;

    private void OnEnable() {
        this.rfp = new RenderFromPool(renderpool);
        data.SubChange(rfp.AddRenderer, rfp.ClearRenderers);
    }
    private void OnDisable() => data.UnsubChange(rfp.AddRenderer, rfp.ClearRenderers);
}
