using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private float projectileDamage;
    private float projectileSpeed;
    private GameObject damageSource;
    public void SetBulletStats(float dmg, float speed, float lifeDuration, GameObject shooter) {
        projectileDamage = dmg;
        projectileSpeed = speed;
        damageSource = shooter;
        GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeDuration);
    }
    private void OnCollisionEnter(Collision other) {
        if(damageSource != null && other.gameObject == damageSource) {
            return;
        }
        other.gameObject.GetComponent<IDamageable>()?.TakeDamage(projectileDamage);
        // Debug.Log($"Projectile hit {other.gameObject.name}");
        Destroy(gameObject);
    }
}
