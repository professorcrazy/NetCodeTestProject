using UnityEngine;

public class MeleeWeapon : IWeapon
{
    public WeaponData Data { get; private set; }
    private float lastAttack;

    public MeleeWeapon(WeaponData data) {  
        this.Data = data;
        lastAttack = -Mathf.Infinity;
    }

    public void Use(GameObject user) {
        if(!CanAttack()) return;

        Vector3 origin = user.transform.position;
        Collider[] hits = Physics.OverlapSphere(origin, Data.range);
        foreach (Collider hit in hits) {
            if (hit.gameObject == user) {
                continue;
            }
            hit.GetComponent<IDamageable>()?.TakeDamage(Data.damage);
            lastAttack = Time.time;
            break;
        }
    }
    private bool CanAttack() {
        if (Data.attackRate <= 0f) return true;
        return Time.time >= lastAttack + (1f / Data.attackRate);
    }
}
