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

    [Header("Throwables Genaral")]
    public float throwForce = 10f;


    public GameObject throwableSpawn;
    public float forceMultiplier = 0f;
    public float forceMultiplierLimit = 2f;

    [Header("Lethals")]
    public int maxLethals = 2;
    public int lethalsCount = 0;
    public Throwable.ThrowableType equippedLethalType;
    public GameObject grenadePrefab;

    [Header("Tactical")]
    public int maxTacticals = 2;
    public int tacticalCount = 0;
    public Throwable.ThrowableType equippedTacticalType;
    public GameObject smokeGrenadePrefab;

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

        equippedLethalType = Throwable.ThrowableType.None;
        equippedTacticalType = Throwable.ThrowableType.None;

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


        if (Input.GetKey(KeyCode.G) || Input.GetKey(KeyCode.T))
        {
            forceMultiplier += Time.deltaTime;

            if (forceMultiplier > forceMultiplierLimit)
            {
                forceMultiplier = forceMultiplierLimit;
            }
        }


        if (Input.GetKeyUp(KeyCode.G))
        {
            if (lethalsCount > 0)
            {
                ThrowLethal();
            }

            forceMultiplier = 0;
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            if (tacticalCount > 0)
            {
                ThrowTactical();
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
                PickupThrowableAsLethal(Throwable.ThrowableType.Grenade);
                break;
            case Throwable.ThrowableType.Smoke_Grenade:
                PickupThrowableAsTactical(Throwable.ThrowableType.Smoke_Grenade);
                break;
        }
    }

    private void PickupThrowableAsTactical(Throwable.ThrowableType tactical)
    {
        if (equippedTacticalType == tactical || equippedTacticalType == Throwable.ThrowableType.None)
        {
            equippedTacticalType = tactical;
            if (tacticalCount < maxTacticals)
            {
                tacticalCount += 1;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();
            }
            else
            {
                print("tactical limit reached");
            }
        }
        else
        {
            //Cannot pick up lethal
            //Option to Swap lethals    
        }
    }

    private void PickupThrowableAsLethal(Throwable.ThrowableType lethal)
    {
        if (equippedLethalType == lethal || equippedLethalType == Throwable.ThrowableType.None)
        {
            equippedLethalType = lethal;
            if (lethalsCount < maxLethals )
            {
                lethalsCount += 1;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();
            }
            else
            {
                print("Lethals limit reached");
            }
        }
        else 
        {
            //Cannot pick up lethal
            //Option to Swap lethals    
        }
    }

    private void ThrowTactical()
    {
        if (grenadePrefab != null && throwableSpawn != null)
        {
            GameObject tacticalPrefab = GetThrowablePrefab(equippedTacticalType);
            GameObject throwable = Instantiate(tacticalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);

            if (throwable != null)
            {
                Rigidbody rb = throwable.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);
                }

                throwable.GetComponent<Throwable>().hasBeenThrown = true;
                tacticalCount -= 1;
                HUDManager.Instance.UpdateThrowablesUI();

                if (tacticalCount <= 0)
                {
                    equippedTacticalType = Throwable.ThrowableType.None;
                }
            }
        }
    }



    private void ThrowLethal()
    {
        if (grenadePrefab != null && throwableSpawn != null)
        {
            GameObject lethalPrefab = GetThrowablePrefab(equippedLethalType);
            GameObject throwable = Instantiate(lethalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);

            if (throwable != null)
            {
                Rigidbody rb = throwable.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);
                }

                throwable.GetComponent<Throwable>().hasBeenThrown = true;
                lethalsCount -= 1;
                HUDManager.Instance.UpdateThrowablesUI();

                if(lethalsCount <= 0)
                {
                    equippedLethalType = Throwable.ThrowableType.None;
                }
            }
        }
    }

    private GameObject GetThrowablePrefab(Throwable.ThrowableType throwableType)
    {
        switch (throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                return grenadePrefab;
            case Throwable.ThrowableType.Smoke_Grenade:
                return smokeGrenadePrefab;
        }
        return new();
    }
}
