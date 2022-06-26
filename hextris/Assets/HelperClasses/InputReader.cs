using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputReader {

    // For convenience
    readonly static KeyCode[] close_keys = new KeyCode[] { KeyCode.Escape };
    readonly static KeyCode[] left_keys = new KeyCode[] { KeyCode.LeftArrow, KeyCode.A };
    readonly static KeyCode[] right_keys = new KeyCode[] { KeyCode.RightArrow, KeyCode.D };
    readonly static KeyCode[] drop_one_keys = new KeyCode[] { KeyCode.S };
    readonly static KeyCode[] drop_full_keys = new KeyCode[] { KeyCode.Space, KeyCode.Return };
    readonly static KeyCode[] rot_cw_keys = new KeyCode[] { KeyCode.UpArrow, KeyCode.C };
    readonly static KeyCode[] rot_ccw_keys = new KeyCode[] { KeyCode.DownArrow, KeyCode.Z };


    // ----------------------------------------------------------------------------------------------------------------
    // Public

    public static bool Close() => ReadKeys(close_keys);

    public static bool DropOne() => ReadKeys(drop_one_keys);
    public static bool DropFull() => ReadKeys(drop_full_keys);

    public static bool Left() => ReadKeys(left_keys);
    public static bool Right() => ReadKeys(right_keys);

    public static bool RotCW() => ReadKeys(rot_cw_keys);
    public static bool RotCCW() => ReadKeys(rot_ccw_keys);


    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    private static bool ReadKeys(KeyCode[] key_codes) {
        bool key_down = false;
        foreach (KeyCode key in key_codes) {
            key_down = Input.GetKeyDown(key);
            if (key_down) break;
        }
        return key_down;
    }
}
