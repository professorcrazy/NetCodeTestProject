using UnityEngine;

public class RangedWeapon : IWeapon
{
    private WeaponData data;
    private int currentAmmo;
    public int CurrentAmmo => currentAmmo;
    private float lastShot;

    public RangedWeapon(WeaponData weaponData, int startingAmmo = -1) {  
        this.data = weaponData;
        currentAmmo = data.ammo;
        lastShot = -Mathf.Infinity;
    }

    public WeaponData Data => data;

    public void Use(GameObject user) {
        if (!CanAttack() || currentAmmo <= 0 || Data.projectilePrefab == null) { 
            return;
        }

        //Spawn positoning
        Transform transform = user.transform;
        Vector3 spawnPos = transform.position + transform.forward + Vector3.up * 1.5f;
        Vector3 direction = transform.forward;
        
        // Spawn the projectile
        var projectileSO = GameObject.Instantiate(data.projectilePrefab, spawnPos, Quaternion.LookRotation(direction));
        var projectile = projectileSO.GetComponent<Projectile>();
        //var rb = projectile.GetComponent<Rigidbody>();

        if (projectile != null) {
            projectile.SetBulletStats(
                dmg: Data.damage, 
                speed: Data.projectileSpeed, 
                lifeDuration: Data.projectileLifetime, 
                shooter: user
            );
            lastShot = Time.time;
            currentAmmo--;
        }
             
        // TODO: Optional callback when out of ammo (like dropping weapon)

    }

    private bool CanAttack() {
        if (Data.attackRate <= 0f) return true;
        return Time.time >= lastShot + (1f / Data.attackRate);
    }
}
