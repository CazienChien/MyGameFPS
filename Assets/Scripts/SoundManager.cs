using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }
    public AudioSource ShootingChannel;
    
    
    public AudioSource emptyMagazineSoundRifle;
    
    public AudioSource reloadingSoundM4A1;
    public AudioSource reloadingSoundDeagle;
    public AudioSource reloadingSoundAK47;

    public AudioClip Deagleshot;
    public AudioClip M4A1shot;
    public AudioClip AK47shot;

    public AudioSource throwablesChannel;
    public AudioClip grenadeSound;
    public AudioClip smokeGrenadeSound;

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
    public void PlayShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.AK47:
                ShootingChannel.PlayOneShot(AK47shot); 
                break;
            case WeaponModel.M4A1:
                ShootingChannel.PlayOneShot(M4A1shot); 
                break;
            case WeaponModel.Deagle:
                ShootingChannel.PlayOneShot(Deagleshot);
                break;
        }
    }

    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.AK47:
                reloadingSoundAK47.Play();
                break;
            case WeaponModel.M4A1:
                reloadingSoundM4A1.Play();
                break;
            case WeaponModel.Deagle:
                reloadingSoundDeagle.Play();
                break;
        }
    }
}

