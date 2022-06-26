using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuckGameState : GameState {

    readonly GSManager parent;

    float end_time_sec;
    const float wait_delay_sec = 2f;

    public StuckGameState(GSManager parent) {
        this.parent = parent;
    }

    public override void Enter() {

        // Signal stuck event
        EventsManager.TriggerStuck();

        // Set end timing & trigger game over sound FX
        end_time_sec = Time.timeSinceLevelLoad + wait_delay_sec;
    }

    public override void Exit() { /* no clean up */ }

    public override void Update() {

        bool end_game = Time.timeSinceLevelLoad > end_time_sec;
        if (end_game) {
            EventsManager.TriggerGameOver();
        }

    }
}
