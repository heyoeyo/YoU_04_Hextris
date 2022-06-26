using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Hextris/Shape Queue")]
public class QueueSO : ScriptableObject {

    [SerializeField] ShapeSO[] allowable_shapes;

    // Holds future spawns
    List<ShapeSO> queue;
    int min_queue_length;


    // ----------------------------------------------------------------------------------------------------------------
    // Public

    public void Init() {
        this.queue = new();
        this.min_queue_length = allowable_shapes.Length;

        RefreshQueue();
    }

    public (ShapeSpec[], Color) GetNext() {
        RefreshQueue();

        ShapeSO first_element = this.queue[0];
        this.queue.RemoveAt(0);

        return ToData(first_element);
    }

    public (ShapeSpec[], Color) GetPreview() {
        ShapeSO preview = this.queue[0];

        return ToData(preview);
    }

    public Color[] GetAllColors() {
        List<Color> all_colors = new();
        foreach (ShapeSO shape in allowable_shapes) {
            all_colors.Add(shape.color);
        }
        return all_colors.ToArray();
    }

    public string[] GetNames() {

        List<string> queue_names = new();
        foreach(ShapeSO shape in this.queue) {
            queue_names.Add(shape.name);
        }

        return queue_names.ToArray();
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    private (ShapeSpec[], Color) ToData(ShapeSO shape) => (shape.shape_specs.ToArray(), shape.color);

    private void RefreshQueue() {

        // Don't do anything if the queue has enough elements in it
        bool queue_is_full = (this.queue.Count >= this.min_queue_length);
        if (queue_is_full) {
            return;
        }

        // Append a sequence of randomized indices, with no doubles, to the queue
        List<ShapeSO> available_shapes = new(allowable_shapes);
        while (available_shapes.Count > 0) {

            // Select a random shape from whatever is available
            int random_idx = Random.Range(0, available_shapes.Count);
            ShapeSO random_shape = available_shapes[random_idx];

            // Add the random shape to the queue & remove from list of potential shapes, so we don't select it again
            this.queue.Add(random_shape);
            available_shapes.RemoveAt(random_idx);
        }

    }


}
