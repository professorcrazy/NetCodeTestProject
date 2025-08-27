using UnityEngine;

public class WeaponPickup : MonoBehaviour
{

    [SerializeField] private WeaponData weaponData;
    [SerializeField] private int currentAmmo;

    public WeaponData WeaponData => weaponData;
    public int CurrentAmmo => currentAmmo;

    public IWeapon CreateWeapon() {
        if (weaponData == null) return null;

        return new RangedWeapon(weaponData, currentAmmo);
    }

    public void SetAmmo(int amount) {
        currentAmmo = amount;
    }
    public void Initialize(WeaponData data, int ammo) {
        weaponData = data;
        currentAmmo = ammo;
    }
}
