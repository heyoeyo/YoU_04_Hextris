using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeKeeper : MonoBehaviour {

    [SerializeField] IntSO game_time_sec;
    [SerializeField] TextMeshProUGUI time_text;

    void Awake() => game_time_sec.Init();

    void Update() => game_time_sec.Set(Mathf.RoundToInt(Time.timeSinceLevelLoad));

    void FixedUpdate() => this.time_text.text = SecondsToMMSS(game_time_sec.Get());

    public static string SecondsToMMSS(int time_sec) {
        // Helper which converts seconds (e.g. 134) to MM:SS format (e.g. 2:14)

        int num_mins = time_sec / 60;
        int num_sec = time_sec % 60;

        return string.Format("{0:00}:{1:00}", num_mins, num_sec);
    }
}
