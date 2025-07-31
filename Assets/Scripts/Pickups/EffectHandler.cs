using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHandler : MonoBehaviour
{
    private float originalSpeed = 5f;
    private float originalDamage = 1f;

    private float currentSpeed;
    private float currentDamage;

    private Dictionary<BoosterEffectSO.BoosterType, Coroutine> activeEffects = new();

    private void Awake() {
        currentSpeed = originalSpeed;
        currentDamage = originalDamage;
    }

    public void ApplyEffect(BoosterEffectSO effect) {
        if (activeEffects.ContainsKey(effect.type)) {
            StopCoroutine(activeEffects[effect.type]);
            ResetEffect(effect.type); // Reset before applying new
        }

        Coroutine routine = StartCoroutine(ApplyTimedEffect(effect));
        activeEffects[effect.type] = routine;
    }

    private IEnumerator ApplyTimedEffect(BoosterEffectSO effect) {
        ApplyBoost(effect);
        yield return new WaitForSeconds(effect.duration);
        ResetEffect(effect.type);
        activeEffects.Remove(effect.type);
    }

    private void ApplyBoost(BoosterEffectSO effect) {
        switch (effect.type) {
            case BoosterEffectSO.BoosterType.Speed:
                currentSpeed += effect.value;
                break;
            case BoosterEffectSO.BoosterType.Damage:
                currentDamage += effect.value;
                break;
        }
    }

    private void ResetEffect(BoosterEffectSO.BoosterType type) {
        switch (type) {
            case BoosterEffectSO.BoosterType.Speed:
                currentSpeed = originalSpeed;
                break;
            case BoosterEffectSO.BoosterType.Damage:
                currentDamage = originalDamage;
                break;
        }
    }

    public float GetSpeed() => currentSpeed;
    public float GetDamage() => currentDamage;
}
