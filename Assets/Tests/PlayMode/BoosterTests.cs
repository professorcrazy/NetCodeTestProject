using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class BoosterTests
{
    private class TestEffectHandler: MonoBehaviour {
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

    [UnityTest]
    public IEnumerator SpeedBooster_IncreasesSpeedTemporarily() {
        // Arrange
        var player = new GameObject("Player");
        var handler = player.AddComponent<TestEffectHandler>();

        var booster = ScriptableObject.CreateInstance<BoosterEffectSO>();
        booster.type = BoosterEffectSO.BoosterType.Speed;
        booster.value = 3f;
        booster.duration = 1f;

        // Act
        handler.ApplyEffect(booster);
        yield return new WaitForSeconds(0.1f); // allow coroutine to start

        // Assert increased speed
        Assert.AreEqual(8f, handler.GetSpeed(), 0.1f, "Started");

        yield return new WaitForSeconds(1.2f); // wait for boost to expire

        // Assert speed resets
        Assert.AreEqual(5f, handler.GetSpeed(), 0.1f, "Boost over");

        Object.Destroy(player);
    }

    //Testing damgeBoost
    public IEnumerator DamageBooster_IncreasesdamageTemporarily() {
        var user = new GameObject("User");
        var handler = user.AddComponent<TestEffectHandler>();

        var booster = ScriptableObject.CreateInstance<BoosterEffectSO>();
        booster.type = BoosterEffectSO.BoosterType.Damage;
        booster.value = 2f;
        booster.duration = 1f;

        handler.ApplyEffect(booster);
        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(3f, handler.GetDamage(), 0.1f);
        yield return new WaitForSeconds(1.2f);
        Assert.AreEqual(1f, handler.GetDamage(), 0.1f);
    }

    //Applying a New Boost Replaces Old One
    [UnityTest]
    public IEnumerator NewBoost_ReplacesOldBoost() {
        var player = new GameObject("Player");
        var handler = player.AddComponent<TestEffectHandler>();

        var first = ScriptableObject.CreateInstance<BoosterEffectSO>();
        first.type = BoosterEffectSO.BoosterType.Speed;
        first.value = 2f;
        first.duration = 5f;

        var second = ScriptableObject.CreateInstance<BoosterEffectSO>();
        second.type = BoosterEffectSO.BoosterType.Speed;
        second.value = 5f;
        second.duration = 1f;

        handler.ApplyEffect(first);
        yield return new WaitForSeconds(0.2f);
        handler.ApplyEffect(second); // should override the first

        Assert.AreEqual(10f, handler.GetSpeed(), 0.1f, "Data1");
        yield return new WaitForSeconds(1.2f);
        Assert.AreEqual(5f, handler.GetSpeed(), 0.1f, "Data2"); // back to base, first one shouldn't persist
    }
}
