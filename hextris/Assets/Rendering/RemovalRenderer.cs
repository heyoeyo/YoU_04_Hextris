using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalRenderer : MonoBehaviour {

    [SerializeField] HexRenderPool renderpool;
    [SerializeField] HexListSO data;
    [SerializeField] float duration_sec = 0.5f;
    [SerializeField] float rotation_amount_deg = 480f;
    [SerializeField] AnimationCurve scale_down;

    RenderFromPool rfp;


    // ----------------------------------------------------------------------------------------------------------------
    // Events

    private void OnEnable() => data.SubChange(rfp.AddRenderer, rfp.ClearRenderers);
    private void OnDisable() => data.UnsubChange(rfp.AddRenderer, rfp.ClearRenderers);


    // ----------------------------------------------------------------------------------------------------------------
    // Built-ins

    private void Awake() {

        // Set up rendering access (this has to happen before OnEnable!)
        this.rfp = new RenderFromPool(renderpool);

        // Disable animation on startup
        this.data.EndAnimating();
    }

    void Update() {

        // Animate only when data signals
        if (data.is_animating) {
            AnimateHexes();
        }

    }


    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    void AnimateHexes() {

        // Figure out animation progress
        float t = this.data.GetAnimationElapsedTime() / this.duration_sec;
        if (t > 1) this.data.EndAnimating();
        t = Mathf.Clamp01(t);

        foreach ((int item_idx, SingleHexData hex_data) in this.data.EnumeratedData()) {
            int render_idx = this.rfp.idxs[item_idx];
            AnimateHex(render_idx, hex_data, t);
        }
    }

    void AnimateHex(int render_index, SingleHexData hex_data, float t) {

        // Set graphics to show changing properties
        float anim_scale = scale_down.Evaluate(t);
        Color anim_color = Color.Lerp(hex_data.color, Color.white, t);
        float anim_rotate = Mathf.Lerp(0, rotation_amount_deg, t);

        rfp.pool.SetGFXScale(render_index, anim_scale);
        rfp.pool.SetGFXColor(render_index, anim_color);
        rfp.pool.SetGFXRotation(render_index, anim_rotate);
    }

}
