using UnityEngine;
namespace Game.Core
{
    public class Health
    {
        public float maxHealth { get; private set; }
        public float currentHealth { get; private set; }
        public bool isDead => currentHealth <= 0;

        public Health(int max) {
            maxHealth = max;
            currentHealth = max;
        }

        public void TakeDamage(float amount) {
            currentHealth = Mathf.Max(0, currentHealth - amount);
        }
        public void Heal(float amount) {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        }
    }
}
