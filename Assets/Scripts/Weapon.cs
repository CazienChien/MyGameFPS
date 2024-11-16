using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
    {
        public bool isActiveWeapon;
        public int weaponDamage;





        [Header("Shooting")]
        // Shooting
        public bool isShooting, readyToShoot;
        bool allowReset = true;
        public float shootingDelay = 2f;
         bool isRecoiling = false; // Thêm biến kiểm soát trạng thái recoil

        [Header("Burst")]
        // Burst
        public int bulletPerBurst = 3;
        public int burstBulletsLeft;

        [Header("Spread")]    
        // Spread
        public float spreadIntensity;
        

        [Header("Bullet")]    
        public GameObject bulletPrefab;
        public Transform bulletSpawn;
        public float bulletVelocity = 30;
        public float bulletPrefabLifeTime = 3f;


        public GameObject muzzleEffect;
        internal Animator animator;

        [Header("Loading")]
        //loading
        public float reloadTime;
        public int magazineSize, bulletsLeft;
        public bool isReloading;

        public Vector3 spawnPosition;
        public Vector3 spawnRotation;

       


    //UI
    public enum WeaponModel
        {
            AK47,
            M4A1,
            Deagle
        }
        public WeaponModel thisWeaponModel;
        public enum ShootingMode
        {
            Single,
            Burst,
            Auto,
        }

        public ShootingMode currentShootingMode;

        private void Awake()
        {
            readyToShoot = true;
            burstBulletsLeft = bulletPerBurst;
            animator = GetComponent<Animator>();
            bulletsLeft = magazineSize;
            
     
    }

        // Update is called once per frame
        void Update()
        {


            if (isActiveWeapon)
            {
               

                

                GetComponent<Outline>().enabled = false;

                if (currentShootingMode == ShootingMode.Auto)
                {
                    // Holding Down Left Mouse Button
                    isShooting = Input.GetKey(KeyCode.Mouse0);
                }
                else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
                {
                    // Clicking Left Mouse Button Once
                    isShooting = Input.GetKey(KeyCode.Mouse0);
                }

                if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
                {
                    Reload();

                }
                if (readyToShoot && isShooting == false && isReloading == false && bulletsLeft <= 0)
                {
                    //Reload();
                }
                if (bulletsLeft == 0 && isShooting)
                {
                    SoundManager.Instance.emptyMagazineSoundRifle.Play();
                }


                // Fire weapon if ready and shooting
                if (readyToShoot && isShooting && bulletsLeft > 0)
                {
                    burstBulletsLeft = bulletPerBurst;
                    FireWeapon();
                }

                // Nếu ngừng bắn và đang trong trạng thái recoil, reset về idle
                if (!isShooting && isRecoiling)
                {
                    animator.SetTrigger("IDLE");
                    isRecoiling = false;
                }


            }

        }
        
      

        private void FireWeapon()
        {
            // Ensure there are bullets left to fire
            if (bulletsLeft <= 0)
            {
                return;
            }

            muzzleEffect.GetComponent<ParticleSystem>().Play();
            //SoundManager.Instance.shootingSoundAK47.Play();

            SoundManager.Instance.PlayShootingSound(thisWeaponModel);

            readyToShoot = false;
            Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

            // Trigger recoil if the gun is not already recoiling
            Debug.Log("Bullet Spawn Position: " + bulletSpawn.position);
           
            
             if(!isRecoiling)
            {
                animator.SetTrigger("RECOIL");
                isRecoiling = true;
                

            }




            // Instantiate the bullet
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
            
            Bullet bul = bullet.GetComponent<Bullet>();
            bul.bulletDamage = weaponDamage;

            // Pointing the bullet to face the shooting direction
            bullet.transform.forward = shootingDirection;
            

            // Shoot
            bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

            // Decrement the bullet count
            bulletsLeft--;

            // Update the UI if needed


            // Destroy bullet after some time
            StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

            // Reset shot after delay
            if (allowReset)
            {
                Invoke("ResetShot", shootingDelay);
                allowReset = false;
            }

            // Handle burst mode
            if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
            {
                burstBulletsLeft--;
                Invoke("FireWeapon", shootingDelay);
            }
        }


        private void Reload()
        {

            SoundManager.Instance.PlayReloadSound(thisWeaponModel);

            animator.SetTrigger("RELOAD");
            isReloading = true;
            Invoke("ReloadCompleted", reloadTime);
        }

        private void ReloadCompleted()
        {
            int ammoLeft = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
            int neededAmmo = magazineSize - bulletsLeft;

            // Nếu còn đủ đạn để nạp đầy băng
            if (ammoLeft >= neededAmmo)
            {
                WeaponManager.Instance.DecreaseTotalAmmo(neededAmmo, thisWeaponModel);
                bulletsLeft = magazineSize;
            }
            else // Nếu không đủ đạn để nạp đầy băng
            {
                bulletsLeft += ammoLeft;
                WeaponManager.Instance.DecreaseTotalAmmo(ammoLeft, thisWeaponModel);
            }

            isRecoiling = false;
            isReloading = false;
        }


        private void ResetShot()
        {
            readyToShoot = true;
            allowReset = true;
        }

        public Vector3 CalculateDirectionAndSpread()
        {

            // Shooting from the middle of the screen to check where we are pointing at
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(100);
            }

            Vector3 direction = targetPoint - bulletSpawn.position;

            
            float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
            float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        // Returning the direction
        return direction + new Vector3(x, y, 0);
            
        }

        private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(bullet);
        }
    }