using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreKeeper : MonoBehaviour {

    [SerializeField] IntSO line_cleared_count;
    [SerializeField] TextMeshProUGUI score_text;


    private void OnEnable() => EventsManager.SubClearRow(IncreaseScore);
    private void OnDisable() => EventsManager.UnsubClearRow(IncreaseScore);


    private void Awake() {
        this.line_cleared_count.Init();
    }

    private void Start() {
        UpdateText();
    }

    void IncreaseScore() {
        line_cleared_count.Increment();
        UpdateText();
    }

    void UpdateText() {
        this.score_text.text = line_cleared_count.ToString();
    }


}
