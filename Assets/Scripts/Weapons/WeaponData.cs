using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
  //public GameObject weaponPrefab;
    public float damage;
    public float range;
    public float attackRate;
    public float cooldown;
    public int ammo;
    public float projectileSpeed;
    public float projectileLifetime;
    public GameObject projectilePrefab;
    public bool isRanged;
}
