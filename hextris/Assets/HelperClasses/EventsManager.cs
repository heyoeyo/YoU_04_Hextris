using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventsManager {

    public static Action PlayerMoveEvent;
    public static Action PlayerRotateEvent;
    public static Action PlayerDropEvent;
    public static Action FastFallEvent;
    public static Action SoftLandEvent;
    public static Action HeavyLandEvent;
    public static Action ClearRowEvent;
    public static Action StuckEvent;
    public static Action GameOverEvent;


    // ----------------------------------------------------------------------------------------------------------------

    public static void SubPlayerInput(Action move_func, Action rotate_func, Action drop_func) {
        PlayerMoveEvent += move_func;
        PlayerRotateEvent += rotate_func;
        PlayerDropEvent += drop_func;
    }
    public static void UnsubPlayerInput(Action move_func, Action rotate_func, Action drop_func) {
        PlayerMoveEvent -= move_func;
        PlayerRotateEvent -= rotate_func;
        PlayerDropEvent -= drop_func;
    }

    public static void TriggerPlayerMove() => PlayerMoveEvent?.Invoke();
    public static void TriggerPlayerRotate() => PlayerRotateEvent?.Invoke();
    public static void TriggerPlayerDrop() => PlayerDropEvent?.Invoke();

    // ----------------------------------------------------------------------------------------------------------------

    public static void SubFastFall(Action func) => FastFallEvent += func;
    public static void UnsubFastFall(Action func) => FastFallEvent -= func;
    public static void TriggerFastFall() => FastFallEvent?.Invoke();

    // ----------------------------------------------------------------------------------------------------------------

    public static void SubSoftLand(Action func) => SoftLandEvent += func;
    public static void UnsubSoftLand(Action func) => SoftLandEvent -= func;
    public static void TriggerSoftLand() => SoftLandEvent?.Invoke();

    // ----------------------------------------------------------------------------------------------------------------

    public static void SubHeavyLand(Action func) => HeavyLandEvent += func;
    public static void UnsubHeavyLand(Action func) => HeavyLandEvent -= func;
    public static void TriggerHeavyLand() => HeavyLandEvent?.Invoke();

    // ----------------------------------------------------------------------------------------------------------------

    public static void SubClearRow(Action func) => ClearRowEvent += func;
    public static void UnsubClearRow(Action func) => ClearRowEvent -= func;
    public static void TriggerClearRow() => ClearRowEvent?.Invoke();

    // ----------------------------------------------------------------------------------------------------------------

    public static void SubStuck(Action func) => StuckEvent += func;
    public static void UnsubStuck(Action func) => StuckEvent -= func;
    public static void TriggerStuck() => StuckEvent?.Invoke();

    // ----------------------------------------------------------------------------------------------------------------

    public static void SubGameOver(Action func) => GameOverEvent += func;
    public static void UnsubGameOver(Action func) => GameOverEvent -= func;
    public static void TriggerGameOver() {
        Debug.Log("DEBUG - GameOver Event");
        GameOverEvent?.Invoke();
    }

}
