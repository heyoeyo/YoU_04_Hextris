using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullShapeMotion : MonoBehaviour {

    Vector2 move_direction;
    float end_time = float.MaxValue;

    public void Init(Vector2 move_direction, float scale, float lifetime_sec) {
        this.transform.localScale = Vector3.one * scale;
        this.move_direction = move_direction;

        // Calculate end time
        this.end_time = lifetime_sec + Time.time;
    }

    void Update() {
        this.transform.Translate(move_direction * Time.deltaTime, Space.World);
        if (Time.time > end_time) {
            Destroy(this.gameObject);
        }
    }
}
