using UnityEngine;

[CreateAssetMenu(fileName = "BoosterEffectSO", menuName = "Pickups/Booster")]
public class BoosterEffectSO : ScriptableObject, IPickup
{
    public enum BoosterType { Speed, Damage }
    public BoosterType type;
    public float value;
    public float duration;

    public string EffectName => type.ToString();

    public void Apply(GameObject target) {
        EffectHandler handler = target.GetComponent<EffectHandler>();
        if (handler != null) {
            handler.ApplyEffect(this);
        }
    }
}
