using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; set; }

    [Header("Ammo")]
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Image ammoTypeUI;

    [Header("Weapon")]
    public Image activeWeaponUI;
    public Image unActiveWeaponUI;

    [Header("Throwables")]
    public Image lethalUI;
    public TextMeshProUGUI lethalAmountUI;

    public Image tacticalUI;
    public TextMeshProUGUI tacticalAmountUI;

    public Sprite emptySlot;

    public GameObject middleDot;
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
    private void Update()
    {
        Weapon activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        Weapon unActiveWeapon = GetUnActiveWeaponSlot().GetComponentInChildren<Weapon>();


        if (activeWeapon)
        {
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft / activeWeapon.bulletPerBurst}";
            totalAmmoUI.text = $"{WeaponManager.Instance.CheckAmmoLeftFor(activeWeapon.thisWeaponModel)}";

            Weapon.WeaponModel model = activeWeapon.thisWeaponModel;
            ammoTypeUI.sprite = GetAmmoSprite(model);

            activeWeaponUI.sprite = GetWeaponSprite(model);

            if (unActiveWeapon)
            {
                unActiveWeaponUI.sprite = GetWeaponSprite(unActiveWeapon.thisWeaponModel);
            }

        }
        else
        {
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";

            ammoTypeUI.sprite = emptySlot;

            activeWeaponUI.sprite= emptySlot;
            unActiveWeaponUI.sprite = emptySlot;
        }



    }

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        GameObject weaponPrefab;
        switch (model)
        {
            case Weapon.WeaponModel.AK47:
                weaponPrefab = Resources.Load<GameObject>("AK47_Weapon");
                break;
            case Weapon.WeaponModel.M4A1:
                weaponPrefab = Resources.Load<GameObject>("M4A1_Weapon");
                break;
            case Weapon.WeaponModel.Deagle:
                weaponPrefab = Resources.Load<GameObject>("Deagle_Weapon");
                break;
            default:
                return null;
        }

        return weaponPrefab != null ? weaponPrefab.GetComponent<SpriteRenderer>().sprite : null;
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        GameObject ammoPrefab;
        switch (model)
        {
            case Weapon.WeaponModel.AK47:
                ammoPrefab = Resources.Load<GameObject>("Rifle_Ammo");
                break;
            case Weapon.WeaponModel.M4A1:
                ammoPrefab = Resources.Load<GameObject>("Rifle_Ammo");
                break;
            case Weapon.WeaponModel.Deagle:
                ammoPrefab = Resources.Load<GameObject>("Pistol_Ammo");
                break;
            default:
                return null;
        }

        return ammoPrefab != null ? ammoPrefab.GetComponent<SpriteRenderer>().sprite : null;
    }


    private GameObject GetUnActiveWeaponSlot()
    {
        foreach (GameObject weaponSlot in WeaponManager.Instance.weaponsSlots)
        {
            if(weaponSlot != WeaponManager.Instance.activeWeaponSlot)
            {
                return weaponSlot;
            }
        }
        return null;
    }

    internal void UpdateThrowables(Throwable.ThrowableType throwable)
    {
        switch (throwable) 
        { 
            case Throwable.ThrowableType.Grenade:
                lethalAmountUI.text = $"{WeaponManager.Instance.grenades}";
                lethalUI.sprite = Resources.Load<GameObject>("Grenade").GetComponent<SpriteRenderer>().sprite;
                break;
        }
    }
}
