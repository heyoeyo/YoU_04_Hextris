using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnder : MonoBehaviour
{
    private void OnEnable() => EventsManager.SubGameOver(LoadGameOver);
    private void OnDisable() => EventsManager.UnsubGameOver(LoadGameOver);

    void LoadGameOver() {
        int gameover_scene_idx = 1 + SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(gameover_scene_idx);
    }
}
