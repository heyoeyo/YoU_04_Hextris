using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockGameState : GameState {

    /* 
     Basic state concept:
     - assign player controlled shape data to (static) grid data
     - clear player shape data
     -> move to clear rows state (under all conditions!)
     */

    readonly GSManager parent;

    public LockGameState(GSManager parent) {
        this.parent = parent;
    }
    public override void Enter() {

        // Copy player hex data over to grid data then clear out player data
        parent.grid.Add(parent.player_hexes);
        parent.player_hexes.Clear();

        // Move to clearing full rows (now that we just added new data!)
        parent.EnterClearRows();
    }

    public override void Exit() { /* no clean up */ }

    public override void Update() { /* no updates */ }
}
