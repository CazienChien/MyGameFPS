using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; set; }

    public List<GameObject> weaponsSlots;

    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalRilfeAmmo = 0;
    public int totalPistolAmmo = 0;

    [Header("Throwable")]
    public int grenades = 0;
    public float throwForce = 10f;
    public GameObject grenadePrefab;
    public GameObject throwableSpawn;
    public float forceMultiplier = 0f;
    public float forceMultiplierLimit = 2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }

    private void Start()
    {
        activeWeaponSlot = weaponsSlots[0];
    }

    private void Update()
    {
        foreach (GameObject weaponSlot in weaponsSlots)
        {
            if (weaponSlot == activeWeaponSlot)
            {
                weaponSlot.SetActive(true);
            }
            else
            {
                weaponSlot.SetActive(false);
            }
        
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchActiveSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchActiveSlot(1);
        }


        if (Input.GetKey(KeyCode.G))
        {
            forceMultiplier += Time.deltaTime;

            if (forceMultiplier > forceMultiplierLimit)
            {
                forceMultiplier = forceMultiplierLimit;
            }
        }


        if (Input.GetKeyUp(KeyCode.G))
        {
            if (grenades > 0)
            {
                ThrowLethal();
            }

            forceMultiplier = 0;
        }
    }

    

    public void PickupWeapon(GameObject pickupWeapon)
    {
        AddWeapoIntoActiveSlot(pickupWeapon);

      
    }

    private void AddWeapoIntoActiveSlot(GameObject pickupWeapon)
    {
        DropCurrentWeapon(pickupWeapon);

        pickupWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        Weapon weapon = pickupWeapon.GetComponent<Weapon>();
        weapon.animator.enabled = true;

        pickupWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);

        pickupWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);

        weapon.isActiveWeapon = true;
    }

    internal void PickupAmmo(AmmoBox ammo)
    {
        switch (ammo.ammoType)
        {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += ammo.ammoAmount;
                break;
            case AmmoBox.AmmoType.RifleAmmo:
                totalRilfeAmmo += ammo.ammoAmount;
                break;
        }
     }

    private void DropCurrentWeapon(GameObject pickupWeapon)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;

            weaponToDrop.GetComponent<Weapon>().isActiveWeapon = false;
            weaponToDrop.GetComponent<Weapon>().animator.enabled = false;

            weaponToDrop.transform.SetParent(pickupWeapon.transform.parent);
            weaponToDrop.transform.localPosition = pickupWeapon.transform.localPosition;
            weaponToDrop.transform.localRotation = pickupWeapon.transform.localRotation;

        }
    }

    public void SwitchActiveSlot(int SlotNumber)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActiveWeapon = false;    
        }

        activeWeaponSlot = weaponsSlots[SlotNumber];

        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
             newWeapon.isActiveWeapon = true    ;
        }
    }

    internal void DecreaseTotalAmmo(int bulletsToDecrease, Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.AK47:
                totalRilfeAmmo -= bulletsToDecrease;
                break;
            case Weapon.WeaponModel.M4A1:
                totalRilfeAmmo -= bulletsToDecrease;
                break;
            case Weapon.WeaponModel.Deagle:
                totalPistolAmmo -= bulletsToDecrease;
                break;
        }
    }
    public int CheckAmmoLeftFor(Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.AK47:
                return totalRilfeAmmo;
            case Weapon.WeaponModel.M4A1:
                return totalRilfeAmmo;
            case Weapon.WeaponModel.Deagle:
                return totalPistolAmmo;
            default:
                return 0;
        }
    }

    // Throwable
    public void PickupThrowable(Throwable throwable)
    {
        switch (throwable.throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                PickupThrowable();
                break;
        }
    }

    private void PickupThrowable()
    {
        grenades += 1;

        HUDManager.Instance.UpdateThrowables(Throwable.ThrowableType.Grenade);

    }


    private void ThrowLethal()
    {
        if (grenadePrefab != null && throwableSpawn != null)
        {
            GameObject lethalPrefab = grenadePrefab;
            GameObject throwable = Instantiate(lethalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);

            if (throwable != null)
            {
                Rigidbody rb = throwable.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);
                }

                throwable.GetComponent<Throwable>().hasBeenThrown = true;
                grenades -= 1;
                HUDManager.Instance.UpdateThrowables(Throwable.ThrowableType.Grenade);
            }
        }
    }
}