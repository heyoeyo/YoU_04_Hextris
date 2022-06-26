using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRestarter : MonoBehaviour
{
    private void Update() {
        bool wait_long_enough = Time.timeSinceLevelLoad > 2;
        if (wait_long_enough && Input.GetKeyDown(KeyCode.Space)) {
            int prev_scene_idx = SceneManager.GetActiveScene().buildIndex - 1;
            SceneManager.LoadScene(prev_scene_idx);
        }
    }
}
