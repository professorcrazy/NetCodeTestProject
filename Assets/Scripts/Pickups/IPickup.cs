using UnityEngine;

public interface IPickup
{
    void Apply(GameObject target);
    string EffectName { get; }
}
