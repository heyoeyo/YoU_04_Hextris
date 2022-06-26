using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitGameState : GameState {

    /* Purpose of this game state is simply to delay game play on initial startup */

    readonly GSManager parent;
    readonly float delay_sec;
    float time_to_leave_state;

    public InitGameState(GSManager parent, float delay_sec) {
        this.parent = parent;
        this.delay_sec = delay_sec;
    }
    public override void Enter() {
        this.time_to_leave_state = Time.timeSinceLevelLoad + this.delay_sec;
    }

    public override void Update() {

        bool leave_state = (Time.timeSinceLevelLoad >= this.time_to_leave_state);
        if (leave_state) {
            parent.EnterClearRows();
        }
    }
    public override void Exit() { /* no clean up */ }
}
