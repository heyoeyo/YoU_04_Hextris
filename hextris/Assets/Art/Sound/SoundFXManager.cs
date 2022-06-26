using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundFXManager : MonoBehaviour {

    [SerializeField] AudioClip player_move_clip;
    [SerializeField] AudioClip player_rotate_clip;
    [SerializeField] AudioClip player_drop_clip;
    [SerializeField] AudioClip fast_fall_clip;
    [SerializeField] AudioClip soft_landing_clip;
    [SerializeField] AudioClip heavy_landing_clip;
    [SerializeField] AudioClip stuck_clip;
    [SerializeField] AudioClip clear_row_clip;

    AudioSource sfx_source;

    // ----------------------------------------------------------------------------------------------------------------
    // Built-ins

    private void Awake() {
        this.sfx_source = this.GetComponent<AudioSource>();
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Events

    private void OnEnable() {
        EventsManager.SubPlayerInput(PlayerMoveSFX, PlayerRotateSFX, PlayerDropSFX);
        EventsManager.SubFastFall(FastFallSFX);
        EventsManager.SubSoftLand(SoftLandingSFX);
        EventsManager.SubHeavyLand(HeavyLandingSFX);
        EventsManager.SubStuck(StuckSFX);
        EventsManager.SubClearRow(ClearRowSFX);
    }

    private void OnDisable() {
        EventsManager.UnsubPlayerInput(PlayerMoveSFX, PlayerRotateSFX, PlayerDropSFX);
        EventsManager.UnsubFastFall(FastFallSFX);
        EventsManager.UnsubSoftLand(SoftLandingSFX);
        EventsManager.UnsubHeavyLand(HeavyLandingSFX);
        EventsManager.UnsubStuck(StuckSFX);
        EventsManager.UnsubClearRow(ClearRowSFX);
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Sound effects functions

    void PlayerMoveSFX() => PlayClip(player_move_clip);
    void PlayerRotateSFX() => PlayClip(player_rotate_clip);
    void PlayerDropSFX() => PlayClip(player_drop_clip);
    void FastFallSFX() => PlayClip(fast_fall_clip);
    void SoftLandingSFX() => PlayClip(soft_landing_clip);
    void HeavyLandingSFX() => PlayClip(heavy_landing_clip);
    void StuckSFX() => PlayClip(stuck_clip);
    void ClearRowSFX() => PlayClip(clear_row_clip);


    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    void PlayClip(AudioClip clip) {
        if (clip != null) {
            sfx_source.PlayOneShot(clip);
        }
    }
}
