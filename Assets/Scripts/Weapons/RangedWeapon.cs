using UnityEngine;

public class RangedWeapon : IWeapon
{
    private WeaponData data;
    private int currentAmmo;
    public int CurrentAmmo => currentAmmo;
    private float lastShot;

    public RangedWeapon(WeaponData weaponData) {  
        this.data = weaponData;
        currentAmmo = data.ammo;
        lastShot = -Mathf.Infinity;
    }

    public WeaponData Data => data;

    public void Use(GameObject user) {
        if (!CanAttack() || currentAmmo <= 0) { 
            return;
        }
        lastShot = Time.time;
        currentAmmo--;

        Transform transform = user.transform;
        Vector3 spawnPos = transform.position + transform.forward + Vector3.up * 1.5f;
        Vector3 direction = transform.forward;
        
        // Spawn the projectile
        var projectile = GameObject.Instantiate(data.projectilePrefab, spawnPos, Quaternion.LookRotation(direction));
        var rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = direction * data.projectileSpeed;

        // Destroy after lifetime
        GameObject.Destroy(projectile, data.projectileLifetime);

        // TODO: Optional callback when out of ammo (like dropping weapon)

    }

    private bool CanAttack() {
        if (Data.attackRate <= 0f) return true;
        return Time.time >= lastShot + (1f / Data.attackRate);
    }
}
