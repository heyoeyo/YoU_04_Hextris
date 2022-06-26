using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class MusicSequencer : MonoBehaviour {

    [SerializeField] BoolSO first_playthrough;
    [SerializeField] AudioClip[] start_clips;
    [SerializeField] AudioClip[] later_clips;

    AudioSource music_source;
    bool enable_music;

    int prev_seq_idx;
    int[] clip_idx_sequence;


    // ----------------------------------------------------------------------------------------------------------------
    // Events

    private void OnEnable() => EventsManager.SubStuck(StopMusic);
    private void OnDisable() => EventsManager.UnsubStuck(StopMusic);
    void StopMusic() {
        this.enable_music = false;
        this.music_source.Stop();
    }

    // ----------------------------------------------------------------------------------------------------------------
    // Built-ins

    void Start() {

        // Set up initial music support
        this.enable_music = true;
        this.music_source = this.GetComponent<AudioSource>();

        // On first play, want to guarantee we play a starter clip (slightly more 'chill' feel than later clips)
        bool play_start_clip = first_playthrough.Get() || (Random.Range(0, 2) > 0);
        if (play_start_clip) PlayRandomClip(start_clips);

        // Make sure to enable later clips from here on out
        first_playthrough.Set(false);

        // Generate shuffled play sequence (so we don't double-up on tracks)
        this.clip_idx_sequence = GetShuffledIndexes(later_clips.Length);
        this.prev_seq_idx = 0;    
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.M)) PlayNextClip();
    }

    void FixedUpdate() {
        if (enable_music && !music_source.isPlaying) {
            PlayNextClip();
        }
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    int[] GetShuffledIndexes(int array_length) {

        // First get all indexes as numbers in a list we can 'deplete'
        List<int> available_idxs = new();
        for (int i = 0; i < array_length; i++) {
            available_idxs.Add(i);
        }

        // Build shuffled indexes by 'picking' from the available indexs (with removal)
        int[] shuffled_idxs = new int[array_length];
        for (int i = 0; i < array_length; i++) {
            int random_available_idx = Random.Range(0, available_idxs.Count);
            shuffled_idxs[i] = available_idxs[random_available_idx];
            available_idxs.RemoveAt(random_available_idx);
        }

        return shuffled_idxs;
    }

    void PlayClip(AudioClip clip) {

        // Play the clip, assuming there is one!
        if (clip != null) {
            this.music_source.clip = clip;
            this.music_source.Play();
            Debug.Log("MUSIC - " + clip.name);

        } else {
            Debug.LogError("Bad clip! (null)");
        }

    }

    void PlayRandomClip(AudioClip[] clips) {
        int random_clip_idx = Random.Range(0, clips.Length);
        PlayClip(clips[random_clip_idx]);
    }

    void PlayNextClip() {
        int next_seq_idx = (this.prev_seq_idx + 1) % this.clip_idx_sequence.Length;
        int clip_idx = this.clip_idx_sequence[next_seq_idx];
        PlayClip(this.later_clips[clip_idx]);

        this.prev_seq_idx = next_seq_idx;
    }
}
