using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyGridRenderer : MonoBehaviour {

    [SerializeField] TileRenderer tile_renderer;
    [SerializeField] HexGFX tile_prefab;
    [SerializeField] bool generate_gfx;

    void Start() {

        // Clear existing graphics (in case we have something from the editor)
        ClearChildObjects();

        // Create grid made of prefabs, combine the meshes and then delete the prefabs
        GridFromPrefab();
        CombineMeshes();
        ClearChildObjects();
    }

    void ClearChildObjects() {
        for (int i = 0; i < this.transform.childCount; i++) {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }

    void GridFromPrefab() {

        // For clarity
        Transform parent = this.transform;

        foreach (GridPosition position in GridShape.AllPositionsIter()) {
            Vector2 render_position = tile_renderer.GetRenderXY(position);
            HexGFX new_tile = Instantiate(tile_prefab, parent);
            new_tile.SetBaseScale(tile_renderer.GetGridScale());
            new_tile.SetPosition(render_position);
        }
    }

    void CombineMeshes() {

        // Clear current mesh data
        this.transform.GetComponent<MeshFilter>().mesh = new Mesh();

        // Loop through all child objects to record mesh data
        CombineInstance[] combined_meshes = new CombineInstance[this.transform.childCount];
        for (int i = 0; i < this.transform.childCount; i++) {
            Transform child_ref = this.transform.GetChild(i);
            combined_meshes[i].mesh = child_ref.GetComponent<MeshFilter>().sharedMesh;
            combined_meshes[i].transform = child_ref.localToWorldMatrix;
        }

        // Combine child meshes into one on the parent object
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combined_meshes);
    }

    [ContextMenu("Generate GFX")]
    private void GenerateGFX() {
        GridFromPrefab();
        CombineMeshes();
    }

}
