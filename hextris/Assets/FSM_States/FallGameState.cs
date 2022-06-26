using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallGameState : GameState {

    /* 
     Basic state concept:
     - reads player input & modify shape position/rotation
     - periodically drops shape, independent of player input. Drop speeds up with game level
     -> move to lock state if shape cannot be dropped any further
     */

    readonly GSManager parent;
    readonly IntSO game_level;
    ResettableTimer timer;

    public FallGameState(GSManager parent, IntSO game_level) {
        this.parent = parent;
        this.game_level = game_level;
    }

    public override void Enter() {

        // Set drop timer, based on current game level
        int curr_level = this.game_level.Get();
        float drop_period_sec = GetDropPeriod(curr_level);
        this.timer = new ResettableTimer(drop_period_sec);

        // Create new drop-indicator hex data
        Color drop_color = new(1, 1, 1, 1f);
        parent.landing_group = new MovableHexGroup(parent.player_group, drop_color);
        SingleHexData[] new_drop_hexes = parent.landing_group.CreateHexData();
        parent.landing_hexes.Replace(new_drop_hexes);
        parent.landing_group.PlaceOnGrid(parent.landing_hexes);
        parent.landing_group.DropFull();
    }

    public override void Exit() {
        // Wipe out the fall preview group once we're finished falling
        parent.landing_group = null;
        parent.landing_hexes.Clear();
    }

    public override void Update() {

        // Check for player full drop command
        if (InputReader.DropFull()) {
            parent.EnterFullFall();
        }

        // Check for player single drop command
        if (InputReader.DropOne()) {
            bool shape_dropped = parent.player_group.Drop();

            // If player manually dropped the shape, prevent automatic drop
            if (shape_dropped) {
                this.timer.Reset();
                EventsManager.TriggerPlayerDrop();
            }
        }

        // Check for player side-to-side movement
        (bool got_move_input, int move_amount) = ReadMove();
        if (got_move_input) {
            bool move_ok = parent.player_group.Move(move_amount);
            parent.landing_group.MatchTo(parent.player_group);
            parent.landing_group.DropFull();

            if (move_ok) EventsManager.TriggerPlayerMove();
        }

        // Check for player rotations
        (bool got_rot_input, int rotation_amount) = ReadRotation();
        if (got_rot_input) {
            bool rotate_ok = parent.player_group.Rotate(rotation_amount);
            parent.landing_group.MatchTo(parent.player_group);
            parent.landing_group.DropFull();

            if (rotate_ok) EventsManager.TriggerPlayerRotate();
        }

        // Have player shape drop automatically, on periodic timer
        bool requires_fall_update = this.timer.Check();
        if (requires_fall_update) {
            bool shape_cant_drop = !parent.player_group.Drop();
            if (shape_cant_drop) {
                EventsManager.TriggerSoftLand();
                parent.EnterLock();
            }
        }

    }

    (bool, int) ReadMove() {

        int move_amount = 0;
        move_amount += InputReader.Left() ? -2 : 0;
        move_amount += InputReader.Right() ? 2 : 0;
        bool got_move_input = move_amount != 0;
        if (got_move_input) {
            //Debug.LogFormat("MOVED: {0}", move_amount);
        }

        return (got_move_input, move_amount);
    }

    (bool, int) ReadRotation() {

        int rot_ccw_amount = 0;
        rot_ccw_amount += InputReader.RotCW() ? -1 : 0;
        rot_ccw_amount += InputReader.RotCCW() ? 1 : 0;
        bool got_rot_input = rot_ccw_amount != 0;
        if (got_rot_input) {
            //Debug.LogFormat("ROTATED: {0}", rot_ccw_amount);
        }

        return (got_rot_input, rot_ccw_amount);
    }

    private float GetDropPeriod(int curr_level) {

        /* Returns a drop period (in seconds) based on the provided current game level */

        // At level 1, drop period is x
        // At level 10, drop period is y
        // At level 100, drop period is z
        // Period maxes out at z

        // For clarity
        const float drop_level_1 = 0.75f;
        const float drop_level_10 = 0.6f;
        const float drop_level_100 = 0.3f;

        // Linearly interpolate between level timings to get current drop timing
        float level_t;
        float drop_period_sec;
        if (curr_level < 10) {
            level_t = (curr_level - 1) / (10 - 1);
            drop_period_sec = Mathf.Lerp(drop_level_1, drop_level_10, level_t);
        } else {
            level_t = Mathf.Clamp01((curr_level - 10) / (100 - 10));
            drop_period_sec = Mathf.Lerp(drop_level_10, drop_level_100, level_t);
        }

        return drop_period_sec;
    }

}
