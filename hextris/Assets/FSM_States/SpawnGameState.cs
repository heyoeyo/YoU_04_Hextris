using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGameState : GameState {

    /*
     Basic state concept:
     - Grab new shape from queue
     - Spawn new cell collection
     - Place cell collection at starting point
     -> Trigger game over if starting placement isn't valid
     -> Move to fall state otherwise
     */

    readonly GSManager parent;
    readonly QueueSO shape_queue;

    public SpawnGameState(GSManager parent, QueueSO shape_queue) {
        this.parent = parent;
        this.shape_queue = shape_queue;
    }

    public override void Enter() {

        // Get new shape (color + cell arrangements) from the queue
        (ShapeSpec[] next_specs, Color next_color) = this.shape_queue.GetNext();
        (ShapeSpec[] queue_specs, Color queue_color) = this.shape_queue.GetPreview();

        // Create new player hex data
        parent.player_group = new MovableHexGroup(next_specs, next_color, parent.grid);
        SingleHexData[] new_player_hexes = parent.player_group.CreateHexData();
        parent.player_hexes.Replace(new_player_hexes);

        // Create new queue hex data
        GridPosition queue_position = GridShape.GetMiddleColumn(0);
        HexGroup queue_group = new(queue_specs, queue_color, queue_position);
        SingleHexData[] new_queue_hexes = queue_group.CreateHexData();
        parent.queue_hexes.Replace(new_queue_hexes);

        // Try to enter shape into the game, if it doesn't fit we enter stuck state (game over)
        bool is_valid_placement = parent.player_group.PlaceOnGrid(parent.player_hexes);
        if (is_valid_placement) {
            parent.EnterFall();
        } else {
            parent.EnterStuck();
        }
    }

    public override void Exit() { /* no clean up */ }

    public override void Update() { /* Nothing to update, we leave state in enter */}
}
