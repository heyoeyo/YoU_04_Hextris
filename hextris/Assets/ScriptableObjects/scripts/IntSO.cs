using UnityEngine;

[CreateAssetMenu(menuName = "Hextris/Integer Data")]
public class IntSO : SharedDataSO {

    [SerializeField] int intial_value;
    [SerializeField] int value;

    public override void Init() => Set(this.intial_value);


    public int Set(int new_value) => this.value = new_value;
    public int Get() => this.value;
    public int Increment(int amount) => this.value += amount;
    public int Increment() => Increment(1);
    public override string ToString() => this.value.ToString();
}