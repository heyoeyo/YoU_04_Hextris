using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndGameReporter : MonoBehaviour {

    [Header("Data references")]
    [SerializeField] IntSO game_level;
    [SerializeField] IntSO lines_cleared;
    [SerializeField] IntSO game_time_sec;

    [Header("UI references")]
    [SerializeField] TextMeshProUGUI level_text;
    [SerializeField] TextMeshProUGUI lines_text;
    [SerializeField] TextMeshProUGUI time_text;

    private void Start() {

        // Update time & level text for gameover
        time_text.text = TimeKeeper.SecondsToMMSS(game_time_sec.Get());
        level_text.text = string.Format("Reached level {0}", game_level.ToString());

        // Display lines cleared, account for plural line/lines
        int num_lines = lines_cleared.Get();
        bool one_line_cleared = (num_lines == 1);
        string lines_msg = one_line_cleared ? "{0} line cleared" : "{0} lines cleared";
        lines_text.text = string.Format(lines_msg, lines_cleared.ToString());
    }

}
