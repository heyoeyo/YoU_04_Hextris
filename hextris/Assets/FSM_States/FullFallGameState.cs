using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullFallGameState : GameState
{

    readonly GSManager parent;
    ResettableTimer timer;

    const float fast_drop_period_sec = 0.02f;

    public FullFallGameState(GSManager parent) {
        this.parent = parent;
    }

    public override void Enter() {
        this.timer = new ResettableTimer(fast_drop_period_sec);
        EventsManager.TriggerFastFall();
    }


    public override void Update() {

        // Only support timer-based drops
        bool requires_fall_update = this.timer.Check();
        if (requires_fall_update) {
            bool shape_cant_drop = !parent.player_group.Drop();
            if (shape_cant_drop) {
                parent.EnterLock();
            }
        }

    }

    public override void Exit() {
        EventsManager.TriggerHeavyLand();
    }
}
