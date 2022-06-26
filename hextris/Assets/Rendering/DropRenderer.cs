using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRenderer : MonoBehaviour {

    [SerializeField] HexRenderPool renderpool;
    [SerializeField] HexListSO data;
    [SerializeField] FloatSO duration_sec;
    [SerializeField] int drop_direction = -1;

    GridPosition drop_offset;
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

        // Figure out drop offset once, so we don't have to keep re-calculating it
        int x_shift = Mathf.RoundToInt(Mathf.Sign(drop_direction));
        this.drop_offset = new GridPosition(x_shift, -1);
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
        float t = this.data.GetAnimationElapsedTime() / this.duration_sec.Get();
        bool end_animation = (t > 1);
        t = Mathf.Clamp01(t);

        foreach ((int item_idx, SingleHexData hex_data) in this.data.EnumeratedData()) {
            int render_idx = this.rfp.idxs[item_idx];
            AnimateHex(render_idx, hex_data, t);
        }

        // Update data positioing to account for drop!
        if (end_animation) {
            foreach ((_, SingleHexData hex_data) in this.data.EnumeratedData()) {
                hex_data.position += this.drop_offset;
            }
            this.data.EndAnimating();
        }
    }

    void AnimateHex(int render_index, SingleHexData hex_data, float t) {
        GridPosition orig_position = hex_data.position;
        GridPosition dest_position = orig_position + this.drop_offset;
        Vector2 anim_position = GridPosition.Lerp(orig_position, dest_position, t);        
        rfp.pool.SetGFXPosition(render_index, anim_position);
    }

}
