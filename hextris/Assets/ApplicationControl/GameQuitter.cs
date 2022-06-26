using UnityEngine;

public class GameQuitter : MonoBehaviour {
    private void Update() {
        if (InputReader.Close()) {
            Application.Quit();
        }
    }

}
