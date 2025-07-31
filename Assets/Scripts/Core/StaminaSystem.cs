using UnityEngine;

public class StaminaSystem
{
    public float maxStamina { get; private set; }
    public float currentStamina { get; private set;}
    public bool isExhausted => currentStamina <= 0f;

    [SerializeField] private float drainRate;
    [SerializeField] private float regenRate;

    public StaminaSystem(float max, float drainRate, float regenRate) {
        maxStamina = max;
        currentStamina = max;
        this.drainRate = drainRate;
        this.regenRate = regenRate;
    }

    public void Update(bool isSprinting, float deltaTime) {
        if (isSprinting && currentStamina > 0) {
            currentStamina -= drainRate * deltaTime;
        }
        else {
            currentStamina += regenRate * deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }
}
