using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullShapeRenderer : MonoBehaviour {

    [SerializeField] ShapeSO shape_def;

    // For clarity
    const string shader_base_color_name = "_BaseColor";

    private void Start() {
        Color shape_color = shape_def.color;
        SetColor(shape_color);
    }


    void SetColor(Color color) {

        // Set material color without creating a new material instance
        Renderer renderer = this.GetComponent<Renderer>();
        MaterialPropertyBlock material_property = new();
        renderer.GetPropertyBlock(material_property);
        material_property.SetColor(shader_base_color_name, color);
        renderer.SetPropertyBlock(material_property);
    }
}
