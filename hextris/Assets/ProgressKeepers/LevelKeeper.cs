using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelKeeper : MonoBehaviour {

    [SerializeField] IntSO game_level;
    [SerializeField] QueueSO coloring;
    [SerializeField] TextMeshProUGUI level_text;
    [SerializeField] float seconds_for_level_1 = 30;

    // Properties associated with custom indicator ring shader
    const string progress_prop_name = "_RingProgress";
    const string fgcolor_prop_name = "_FgColor";
    const string bgcolor_prop_name = "_BgColor";
    int _progress_id;
    int _fgcolor_id;
    int _bgcolor_id;

    // Used to update level indicators
    Material mat;
    Color[] color_seq;
    int prev_fg_idx = -1;
    int prev_bg_idx = -1;

    // Used to keep track of level timing
    float curr_level_duration;
    float level_start_time;
    float seconds_for_prev_level;

    // Defined so we don't keep re-instantiating these
    float curr_time;
    float level_progress;


    // ----------------------------------------------------------------------------------------------------------------
    // Built-ins

    private void Awake() {
        this.game_level.Init();
    }

    private void Start() {

        // Set up referencing to custom ring shader, used to indicate level progress
        this.mat = this.GetComponent<MeshRenderer>().sharedMaterial;
        this._progress_id = Shader.PropertyToID(progress_prop_name);
        this._fgcolor_id = Shader.PropertyToID(fgcolor_prop_name);
        this._bgcolor_id = Shader.PropertyToID(bgcolor_prop_name);

        // Get Coloring for background of ring indicator
        this.color_seq = this.coloring.GetAllColors();

        // Assuming first level starts on load!
        this.level_start_time = Time.timeSinceLevelLoad;

        // Set up initial 'fibonacci' timing
        this.seconds_for_prev_level = 0;
        this.curr_level_duration = seconds_for_level_1;

        // Set up initial indicator status
        SetLevelText();
        SetLevelProgress(0);
        SetIndicatorColors();
        SetNextLevelDuration();
    }

    private void Update() {

        // Update level after duration ends
        curr_time = Time.timeSinceLevelLoad;
        level_progress = (curr_time - this.level_start_time) / this.curr_level_duration;
        if (level_progress > 1) {
            IncreaseLevel(curr_time);
        }

        // Update indicator bar progress
        SetLevelProgress(level_progress);
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    void IncreaseLevel(float current_time) {

        // Update timing
        this.level_start_time = current_time;
        SetNextLevelDuration();

        // Update level indicator
        this.game_level.Increment();
        SetLevelText();
        SetIndicatorColors();

        // Reset level progress for better UI updating
        level_progress = 0.0f;
    }

    void SetLevelText() => this.level_text.text = this.game_level.ToString();

    void SetLevelProgress(float level_progress) => this.mat.SetFloat(this._progress_id, level_progress);

    void SetNextLevelDuration() {
        // Update using fibonannci-ish time increases
        float new_seconds_for_prev_level = this.curr_level_duration;
        this.curr_level_duration += seconds_for_prev_level / 4.0f;
        this.seconds_for_prev_level = new_seconds_for_prev_level;
    }

    void SetIndicatorColors() {

        // For convenience
        int num_colors = this.color_seq.Length;

        // Set initial previous fg color if not already set
        if (this.prev_fg_idx < 0) {
            this.prev_fg_idx = Random.Range(0, num_colors);
        }

        // Swap old fg to be bg, then pick new new random fg (without duplicating fg/bg)
        int bg_idx = this.prev_fg_idx;
        int fg_idx = (bg_idx + Random.Range(1, num_colors)) % num_colors;

        // Handle special case where new foreground swaps with old background
        // (just looks funny, so stop it from happening)
        if (fg_idx == this.prev_bg_idx) {
            fg_idx = (fg_idx + 1) % num_colors;
        }

        // Update shader properties
        Color fg = this.color_seq[fg_idx];
        Color bg = this.color_seq[bg_idx];
        this.mat.SetColor(this._fgcolor_id, fg);
        this.mat.SetColor(this._bgcolor_id, bg);

        // Update 'previous' colors for next iteration
        this.prev_fg_idx = fg_idx;
        this.prev_bg_idx = bg_idx;
    }

}
