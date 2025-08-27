using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] private GameObject weaponPickupPrefab;
    private IWeapon currentWeapon;

    public void EquipWeapon(IWeapon newWeapon) {
        currentWeapon = newWeapon;
        //TODO: Trigger events, play sound or other effects when getting a new weapon
    }

    public void UseWeapon() {
        currentWeapon?.Use(gameObject);
    }
    public void DropCurrentWeapon() {
        if (currentWeapon == null || weaponPickupPrefab == null) {
            return;
        }

        if (currentWeapon is RangedWeapon ranged) {
            Vector3 dropPosition = transform.position + transform.forward;
            GameObject pickupGO = Instantiate(weaponPickupPrefab, dropPosition, Quaternion.identity);
            WeaponPickup pickup = pickupGO.GetComponent<WeaponPickup>();

            if (pickup != null) {
                pickup.Initialize(ranged.Data, ranged.CurrentAmmo);
            }
        }
        currentWeapon = null;
    }
    public bool HasWeapon => currentWeapon != null;
}
