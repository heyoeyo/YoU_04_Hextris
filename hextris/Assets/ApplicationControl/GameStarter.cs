using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour {

    [SerializeField] BoolSO first_playthrough;

    private void Start() {
        first_playthrough.Init();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            int next_scene_idx = 1 + SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(next_scene_idx);
        }
    }
}
