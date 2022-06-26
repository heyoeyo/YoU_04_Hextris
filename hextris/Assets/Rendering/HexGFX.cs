using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGFX : MonoBehaviour
{
    // For clarity
    const string shader_base_color_name = "_BaseColor";
    Color color;
    float base_scale = 1f;

    public void SetBaseScale(float base_scale) {
        this.base_scale = base_scale;
        SetScale(1);
    }

    public void SetScale(float scale) => this.transform.localScale = (base_scale * scale) * Vector3.one;
    public void SetPosition(Vector3 position) => this.transform.position = position;
    public void SetRotation(float rotation_deg) => this.transform.rotation = Quaternion.Euler(0, 0, rotation_deg);

    public void SetColor(Color color) {

        // Don't do anything if we already have the correct color
        if (this.color == color) {
            return;
        }

        // Store for future checks
        this.color = color;

        // Set material color without creating a new material instance
        Renderer renderer = this.GetComponent<Renderer>();
        MaterialPropertyBlock material_property = new();
        renderer.GetPropertyBlock(material_property);
        material_property.SetColor(shader_base_color_name, color);
        renderer.SetPropertyBlock(material_property);
    }
}
