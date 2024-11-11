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
}
