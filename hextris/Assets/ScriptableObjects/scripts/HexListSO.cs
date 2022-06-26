using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Hextris/Hexlist")]
public class HexListSO : ScriptableObject {

    readonly private List<SingleHexData> list = new();
    private float animation_start_time;
    [HideInInspector] public bool is_animating = false;
    [HideInInspector] public List<GridPosition> init_mask = new();


    // ----------------------------------------------------------------------------------------------------------------
    // Events

    public Action<SingleHexData> HexAdded;
    public Action HexesCleared;
    public void SubChange(Action<SingleHexData> OnAdd, Action OnClear) {
        HexAdded += OnAdd;
        HexesCleared += OnClear;
    }

    public void UnsubChange(Action<SingleHexData> OnAdd, Action OnClear) {
        HexAdded -= OnAdd;
        HexesCleared -= OnClear;
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Public - Data access

    // This makes the class behave as though it is an array-like that can be indexed directly!
    public SingleHexData this[int index] {
        get => list[index];
        set => list[index] = value;
    }

    public IEnumerable<(int, SingleHexData)> EnumeratedData() {

        int index = 0;
        foreach(SingleHexData item in list) {
            yield return (index, item);
            index++;
        }
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Public - Data modifiers

    public void Clear() {
        this.list.Clear();
        HexesCleared?.Invoke();
    }

    public void Add(SingleHexData[] new_data) {
        foreach(SingleHexData item in new_data) {
            this.list.Add(item);
            HexAdded?.Invoke(item);
        }
    }

    public void Replace(SingleHexData[] new_data) {
        Clear();
        Add(new_data);
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Animation control

    public void BeginAnimating() {

        // Bail on animation if we don't have data!
        bool no_data = (this.list.Count == 0);
        if (no_data) {
            this.EndAnimating();
            return;
        }

        this.animation_start_time = Time.timeSinceLevelLoad;
        this.is_animating = true;
    }

    public void EndAnimating() {
        this.animation_start_time = -1;
        this.is_animating = false;
    }

    public float GetAnimationElapsedTime() {
        return (Time.timeSinceLevelLoad - this.animation_start_time);
    }

}
