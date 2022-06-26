using UnityEngine;

[CreateAssetMenu(menuName = "Hextris/Float Data")]
public class FloatSO : SharedDataSO {

    [SerializeField] float initial_value;
    [SerializeField] float value;

    public override void Init() => Set(this.initial_value);

    public float Set(float new_value) => this.value = new_value;
    public float Get() => this.value;
}