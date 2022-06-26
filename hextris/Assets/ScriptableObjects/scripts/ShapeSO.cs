using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hextris/Shape Data")]
public class ShapeSO : ScriptableObject {

    public Color color;
    public List<ShapeSpec> shape_specs;

    public void AddOrientation(ShapeSpec new_spec) {

        // Make sure we have to something to work with
        if (shape_specs == null) {
            shape_specs = new();
        }

        // Add new spec (note: duplicates are allowed!)
        shape_specs.Add(new_spec);
    }

}