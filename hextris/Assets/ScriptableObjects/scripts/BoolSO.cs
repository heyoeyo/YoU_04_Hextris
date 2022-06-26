using UnityEngine;

[CreateAssetMenu(menuName = "Hextris/Boolean Data")]
public class BoolSO : SharedDataSO {

    [SerializeField] bool intial_value;
    [SerializeField] bool value;

    public override void Init() => Set(this.intial_value);

    public bool Set(bool new_value) => this.value = new_value;
    public bool Get() => this.value;
    public override string ToString() => this.value.ToString();
}