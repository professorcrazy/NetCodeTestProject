using UnityEngine;

public interface IWeapon
{
    void Use(GameObject user);
    WeaponData Data { get; }
}
