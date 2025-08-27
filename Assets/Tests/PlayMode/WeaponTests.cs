using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class WeaponTests
{
    private class DummyTarget : MonoBehaviour, IDamageable
    {
        public float Health = 100f;
        public void TakeDamage(float amount, GameObject source = null) => Health -= amount;
    }


    /// <summary>
    /// Melee Weapon test
    /// </summary>

    //Checks damage calcucaltion 
    [UnityTest]
    public IEnumerator MeleeWeapon_DealsCorrectDamage() {
        // Arrange
        var data1 = ScriptableObject.CreateInstance<WeaponData>();
        data1.damage = 20f;
        data1.range = 2f;
        data1.isRanged = false;
        data1.attackRate = 999f;

        var dummy1 = new GameObject("DummyTarget1").AddComponent<DummyTarget>();
        dummy1.transform.position = new Vector3(0, 0, 11);
        dummy1.gameObject.AddComponent<SphereCollider>();

        var user1 = new GameObject("Attacker1");
        user1.transform.position = new Vector3(0,0,10);

        yield return new WaitForSeconds(0.1f);
        var weapon1 = new MeleeWeapon(data1);
        weapon1.Use(user1);
        Assert.AreEqual(80f, dummy1.Health);
        UnityEngine.MonoBehaviour.Destroy(user1.gameObject);
        UnityEngine.MonoBehaviour.Destroy(dummy1.gameObject);
    }

    //do not hit if target is out of range
    [UnityTest]
    public IEnumerator MeleeWeapon_OutOfRange() {
        // Arrange
        var data1 = ScriptableObject.CreateInstance<WeaponData>();
        data1.damage = 20f;
        data1.range = 1f;
        data1.isRanged = false;
        data1.attackRate = 999f;

        var dummy1 = new GameObject("DummyTarget1").AddComponent<DummyTarget>();
        dummy1.transform.position = new Vector3(0, 0, 11.9f);
        dummy1.gameObject.AddComponent<SphereCollider>();

        var user1 = new GameObject("Attacker1");
        user1.transform.position = new Vector3(0, 0, 10);

        yield return new WaitForSeconds(0.1f);
        var weapon1 = new MeleeWeapon(data1);
        weapon1.Use(user1);
        Assert.AreEqual(100f, dummy1.Health);
        UnityEngine.MonoBehaviour.Destroy(user1.gameObject);
        UnityEngine.MonoBehaviour.Destroy(dummy1.gameObject);
    }

    //Weapon Range 0 - Shoudl not hit or produce an error
    [UnityTest]
    public IEnumerator MeleeWeapon_WeaponRangeZero() {
        // Arrange
        var data = ScriptableObject.CreateInstance<WeaponData>();
        data.damage = 20f;
        data.range = 0f;
        data.isRanged = false;
        data.attackRate = 999f;

        var dummy = new GameObject("DummyTarget1").AddComponent<DummyTarget>();
        dummy.transform.position = new Vector3(0, 0, 11.9f);
        dummy.gameObject.AddComponent<SphereCollider>();

        var user = new GameObject("Attacker1");
        user.transform.position = new Vector3(0, 0, 10);

        yield return new WaitForSeconds(0.1f);
        var weapon = new MeleeWeapon(data);
        weapon.Use(user);
        Assert.AreEqual(100f, dummy.Health);
        UnityEngine.MonoBehaviour.Destroy(user.gameObject);
        UnityEngine.MonoBehaviour.Destroy(dummy.gameObject);
    }

    //No Valid targets - Shoudl not hit or produce an error
    [UnityTest]
    public IEnumerator MeleeWeapon_NoValidTargets() {
        // Arrange
        var data = ScriptableObject.CreateInstance<WeaponData>();
        data.damage = 20f;
        data.range = 0f;
        data.isRanged = false;
        data.attackRate = 999f;

        var dummy = new GameObject("DummyTarget1");
        dummy.transform.position = new Vector3(0, 0, 11.9f);
        dummy.gameObject.AddComponent<SphereCollider>();

        var user = new GameObject("Attacker1");
        user.transform.position = new Vector3(0, 0, 10);

        yield return new WaitForSeconds(0.1f);
        var weapon = new MeleeWeapon(data);
        weapon.Use(user);
        Assert.Pass("Weapon ignored non-damageable target without crashing.");
        UnityEngine.MonoBehaviour.Destroy(user.gameObject);
        UnityEngine.MonoBehaviour.Destroy(dummy.gameObject);
    }

    //Do not Hit yourself
    [UnityTest]
    public IEnumerator MeleeWeapon_SelfAfflictedDamage() {
        // Arrange
        var data = ScriptableObject.CreateInstance<WeaponData>();
        data.damage = 20f;
        data.range = 0f;
        data.isRanged = false;
        data.attackRate = 999f;

        var dummy = new GameObject("UnvalidTarget");
        dummy.transform.position = new Vector3(0, 0, 11.9f);
        dummy.gameObject.AddComponent<SphereCollider>();

        var user = new GameObject("Attacker1").AddComponent<DummyTarget>();
        user.transform.position = new Vector3(0, 0, 10);

        yield return new WaitForSeconds(0.1f);

        var weapon = new MeleeWeapon(data);
        weapon.Use(user.gameObject);

        Assert.AreEqual(100f, user.Health);
        UnityEngine.MonoBehaviour.Destroy(user.gameObject);
        UnityEngine.MonoBehaviour.Destroy(dummy.gameObject);
    }

    //Tests Cooldown for both fails and after a delay
    [UnityTest]
    public IEnumerator MeleeWeapon_CannotAttackDuringCooldown() {
        var data2 = ScriptableObject.CreateInstance<WeaponData>();
        data2.damage = 30f;
        data2.range = 2f;
        data2.attackRate = 1f;

        var dummy2 = new GameObject("DummyTarget2").AddComponent<DummyTarget>();
        dummy2.transform.position = Vector3.forward * 1f;
        dummy2.gameObject.AddComponent<SphereCollider>();

        var user2 = new GameObject("Attacker2");
        user2.transform.position = Vector3.zero;

        var weapon2 = new MeleeWeapon(data2);
        yield return null;
        weapon2.Use(user2);
        Assert.AreEqual(70f, dummy2.Health);

        yield return new WaitForSeconds(0.1f);
        weapon2.Use(user2);
        Assert.AreEqual(70f, dummy2.Health);

        yield return new WaitForSeconds(1.1f);
        weapon2.Use(user2);
        Assert.AreEqual(40f, dummy2.Health);
        UnityEngine.MonoBehaviour.Destroy(user2.gameObject);
        UnityEngine.MonoBehaviour.Destroy(dummy2.gameObject);
    }

    //Testing if you can hit multiple enemies (currently only one target is allowed) 
    [UnityTest]
    public IEnumerator MeleeWeapon_CanHitOnlyOne() {
        var data = ScriptableObject.CreateInstance<WeaponData>();
        data.damage = 30f;
        data.range = 2f;
        data.attackRate = 1f;


        var dummy1 = new GameObject("DummyTarget1").AddComponent<DummyTarget>();
        dummy1.transform.position = Vector3.forward * 1f;
        dummy1.gameObject.AddComponent<SphereCollider>();

        var dummy2 = new GameObject("DummyTarget2").AddComponent<DummyTarget>();
        dummy2.transform.position = Vector3.forward * 1f;
        dummy2.gameObject.AddComponent<SphereCollider>();

        var dummy3 = new GameObject("DummyTarget3").AddComponent<DummyTarget>();
        dummy3.transform.position = Vector3.forward * 1f;
        dummy3.gameObject.AddComponent<SphereCollider>();

        var user = new GameObject("Attacker2");
        user.transform.position = Vector3.zero;

        var weapon = new MeleeWeapon(data);
        yield return null;
        weapon.Use(user);

        int damagedCount = 0;
        if (dummy1.Health < 100) damagedCount++;
        if (dummy2.Health < 100) damagedCount++;
        if (dummy3.Health < 100) damagedCount++;

        Assert.AreEqual(1, damagedCount);
        
        UnityEngine.MonoBehaviour.Destroy(user.gameObject);
        UnityEngine.MonoBehaviour.Destroy(dummy1.gameObject);
        UnityEngine.MonoBehaviour.Destroy(dummy2.gameObject);
        UnityEngine.MonoBehaviour.Destroy(dummy3.gameObject);
    }

    /// <summary>
    /// Ranged Weapon test
    /// </summary>
    //Shoot once with 3 projectiles available 
    [UnityTest]
    public IEnumerator RangedWeapon_SpawnsProjectileAndConsumesAmmo() {
        var data = ScriptableObject.CreateInstance<WeaponData>();
        data.isRanged = true;
        data.ammo = 1;
        data.damage = 10;
        data.attackRate = 999f;
        data.projectileSpeed = 20f;
        data.projectileLifetime = 3f;


        data.projectilePrefab = new GameObject("Projectile");
        data.projectilePrefab.AddComponent<Rigidbody>();
        data.projectilePrefab.AddComponent<Projectile>();

        var weapon = new RangedWeapon(data);
        var user = new GameObject("Shooter");
        user.transform.position = new Vector3(-30f, 0, 0);
        user.transform.forward = Vector3.forward;

        yield return null;
        weapon.Use(user);

        yield return null;
        var spawned = GameObject.Find("Projectile");

        Assert.NotNull(spawned, "Projectile should be spawned.");
        Assert.AreEqual(2, weapon.CurrentAmmo); // Assuming internal ammo is updated

        // Cleanup
        UnityEngine.Object.Destroy(spawned.gameObject);
        UnityEngine.Object.Destroy(data);
        UnityEngine.Object.Destroy(user.gameObject);
    }
    [TearDown]
    public void CleanUpProjectiles() {
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>()) {
            if (obj.name.Contains("Projectile"))
                GameObject.Destroy(obj);
        }
    }
    //Shoot twice with 2 projectile available 
    [UnityTest]
    public IEnumerator RangedWeapon_ShootTwiceProjectileAndConsumesAmmo() {
        yield return new WaitForSeconds(0.1f);
        CleanUpProjectiles();
        var data = ScriptableObject.CreateInstance<WeaponData>();
        data.isRanged = true;
        data.ammo = 1;
        data.damage = 10;
        data.attackRate = 999f;
        data.projectileSpeed = 20f;
        data.projectileLifetime = 3f;


        data.projectilePrefab = new GameObject("Projectile");
        data.projectilePrefab.AddComponent<Rigidbody>();
        data.projectilePrefab.AddComponent<Projectile>();

        var weapon = new RangedWeapon(data);
        var user = new GameObject("Shooter");
        user.transform.position = new Vector3(-30f, 0, 0);
        user.transform.forward = Vector3.forward;

        yield return null;
        weapon.Use(user);

        yield return null;
        // Count how many projectiles exist
        var projectiles = GameObject.FindGameObjectsWithTag("Ammo"); // or use a real tag later
        int initialCount = projectiles.Length;

//        Assert.AreEqual(1, initialCount, "Test 1: A projectile should have been spawned.");
        //Assert.AreEqual(1, weapon.CurrentAmmo, "Test 1: one remaining arrow");
        yield return new WaitForSeconds(0.2f);
        weapon.Use(user); // Try to fire again with 0 ammo

        yield return null;

        var projectilesAfter = GameObject.FindGameObjectsWithTag("Ammo");
        int finalCount = projectilesAfter.Length;

//        Assert.AreEqual(2, finalCount, "Test 1: A second projectile shoud have been shot");
        Assert.AreEqual(0, weapon.CurrentAmmo, "Test 1: no remaining arrow");

        // Cleanup
        foreach (var go in projectilesAfter)
            UnityEngine.Object.Destroy(go.gameObject);

        UnityEngine.Object.Destroy(data);
        UnityEngine.Object.Destroy(user);
    }

    //Shoot twice with 1 projectile available 
    [UnityTest]
    public IEnumerator RangedWeapon_ShootTwiceProjectileAndConsumesAmmoFail() {
        yield return new WaitForSeconds(0.3f);
        CleanUpProjectiles();
        var data = ScriptableObject.CreateInstance<WeaponData>();
        data.isRanged = true;
        data.ammo = 1;
        data.damage = 10;
        data.attackRate = 999f;
        data.projectileSpeed = 20f;
        data.projectileLifetime = 3f;


        data.projectilePrefab = new GameObject("Projectile");
        data.projectilePrefab.AddComponent<Rigidbody>();
        data.projectilePrefab.AddComponent<Projectile>();


        var weapon = new RangedWeapon(data);
        var user = new GameObject("Shooter");
        user.transform.position = new Vector3(-30f,0,0);
        user.transform.forward = Vector3.forward;

        yield return null;
        weapon.Use(user);

        yield return null;
        // Count how many projectiles exist
        var projectiles = GameObject.FindGameObjectsWithTag("Ammo2"); // or use a real tag later
        int initialCount = projectiles.Length;

        Assert.Greater(initialCount, 0, "Test2: A projectile should have been spawned.");
        yield return new WaitForSeconds(0.2f);
        weapon.Use(user); // Try to fire again with 0 ammo

        yield return null;

        var projectilesAfter = GameObject.FindGameObjectsWithTag("Ammo2");
        int finalCount = projectilesAfter.Length;

        Assert.AreEqual(initialCount, finalCount, "No additional projectile should have spawned.");

        // Cleanup
        foreach (var go in projectilesAfter)
            UnityEngine.Object.Destroy(go);

        UnityEngine.Object.Destroy(data);
        UnityEngine.Object.Destroy(user);
    }
 
    [UnityTest]
    public IEnumerator RangedWeapon_RespectsCooldownBetweenShots() {
        yield return new WaitForSeconds(0.5f);
        var data = ScriptableObject.CreateInstance<WeaponData>();
        data.isRanged = true;
        data.ammo = 2;
        data.damage = 10;
        data.attackRate = 1f; // 1 shot per second
        data.projectileSpeed = 10f;
        data.projectileLifetime = 2f;
        data.projectilePrefab = new GameObject("Projectile");
        data.projectilePrefab.AddComponent<Rigidbody>();
        data.projectilePrefab.AddComponent<Projectile>();
        var user = new GameObject("Shooter");
        user.transform.forward = Vector3.forward;

        var weapon = new RangedWeapon(data);

        // First fire
        weapon.Use(user);
        float ammoAfterFirst = weapon.CurrentAmmo;

        // Fire again immediately
        weapon.Use(user);
        float ammoAfterSecond = weapon.CurrentAmmo;

        Assert.AreEqual(ammoAfterFirst, ammoAfterSecond, "Should not have consumed ammo during cooldown");

        yield return null;

        GameObject.Destroy(user);
        GameObject.Destroy(data.projectilePrefab);
    }
 }