using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResettableTimer {

    float period_sec;
    float next_trigger_time_sec;

    public ResettableTimer(float period_sec) {
        this.period_sec = period_sec;
        Reset();
    }

    public void Reset() => this.next_trigger_time_sec = Time.timeSinceLevelLoad + this.period_sec;

    public bool Check() {

        /* Check if the timer is up */

        float curr_time = Time.timeSinceLevelLoad;
        bool timer_up = Time.timeSinceLevelLoad > this.next_trigger_time_sec;
        if (timer_up) {
            this.next_trigger_time_sec += this.period_sec;
        }

        return timer_up;
    }

}
