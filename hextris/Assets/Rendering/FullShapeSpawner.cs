using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullShapeSpawner : MonoBehaviour {


    [SerializeField] FullShapeMotion[] shape_prefabs;
    [SerializeField] Vector2 spawn_range_origin = new(0, 0);
    [SerializeField] float spawn_y_height = 10;
    [SerializeField] float move_angle_deg = 45;
    [SerializeField] float move_speed = 5;
    [SerializeField] float spawn_scale = 0.5f;
    [SerializeField] float spawn_period_sec = 2f;
    [SerializeField] float spawn_lifetime_sec = 5;
    [SerializeField, Range(3, 10)] int spawn_count = 6;


    float next_spawn_time_sec = float.MinValue;

    private void Update() {

        float curr_time = Time.timeSinceLevelLoad;
        if (curr_time > next_spawn_time_sec) {
            next_spawn_time_sec = curr_time + spawn_period_sec;

            foreach(Vector2 position in SpawnPositionIter()) {
                SpawnShape(position);

            }

        }

    }

    private void SpawnShape(Vector2 spawn_position) {

        // Pick a random prefab to spawn
        int random_idx = Random.Range(0, shape_prefabs.Length);
        FullShapeMotion prefab = shape_prefabs[random_idx];
        Quaternion spawn_rotation = Quaternion.Euler(0, 0, 60 * Random.Range(0, 5));
        FullShapeMotion new_shape = Instantiate(prefab, spawn_position, spawn_rotation, this.transform);

        // Setup shape
        Vector2 move_direction = GetMoveDirection();
        new_shape.Init(move_direction, spawn_scale, spawn_lifetime_sec);
    }

    private Vector2 GetMoveDirection() {
        float move_angle_rad = move_angle_deg * Mathf.Deg2Rad;
        Vector2 move_direction = new Vector2(Mathf.Cos(move_angle_rad), Mathf.Sin(move_angle_rad)) * move_speed;
        return move_direction;
    }

    private IEnumerable<Vector2> SpawnPositionIter() {

        (Vector2 pt1, Vector2 pt2) = GetSpawnEndPoints();
        Vector2 pt_delta = pt2 - pt1;

        for (int i = 0; i < spawn_count; i++) {
            float t = (float) i/ (float) Mathf.Max(spawn_count, 1);
            Vector2 spawn_position = pt1 + t * pt_delta;
            yield return spawn_position;
        }
    }

    private (Vector2, Vector2) GetSpawnEndPoints() {

        Vector2 pt1 = new(spawn_range_origin.x, spawn_range_origin.y);
        Vector2 pt2 = pt1 + Vector2.up * spawn_y_height;

        return (pt1, pt2);
    }

    private void OnDrawGizmos() {

        (Vector2 pt1, Vector2 pt2) = GetSpawnEndPoints();
        Gizmos.DrawLine(pt1, pt2);

        // Calculate movement distance
        Vector2 move_direction = GetMoveDirection();
        Vector2 move_delta = move_direction * spawn_lifetime_sec;


        Vector3 spawn_size = Vector3.one * 0.2f;
        foreach(Vector2 position in SpawnPositionIter()) {

            // Draw line indicating spawn travel path + spawn point
            Vector2 end_position = position + move_delta;
            Gizmos.DrawLine(position, end_position);
            Gizmos.DrawCube(position, spawn_size);
        }
    }
}

