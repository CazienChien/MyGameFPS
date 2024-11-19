using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TreeEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieSpawnController : MonoBehaviour
{
    public int initialZombiePerWave = 2;
    public int currentZombiesPerWave;

    public float spawnDelay = 0.5f; //Delay between spawning each zombie in a wave

    public int currentWave = 0;
    public float waveCooldown = 10.0f; // Cooldown between waves

    public bool inCooldown;
    public float cooldownCounter = 0; // use for testing

    public List<Enemy> currentZombiesAlive;

    public GameObject zombiePrefab;
    public GameObject fastZombiePrefab;
    public GameObject strongZombiePrefab;

    public TextMeshProUGUI waveOverUI;
    public TextMeshProUGUI cooldownCounterUI;

    public TextMeshProUGUI currentWaveUI;

    private int zombieTypeChangeInterval = 3; // Every 3 waves, change zombie type
    private int zombieTypeIndex = 0; // 0 = Default, 1 = Fast, 2 = Strong
    private void Start()
    {
        currentZombiesPerWave = initialZombiePerWave;

        GlobalReferences.Instance.waveNumber = currentWave;

        StartNextWave();
    }


    private void StartNextWave()
    {
        currentZombiesAlive.Clear();

        currentWave++;

        int newZombieTypeIndex = (currentWave - 1) / zombieTypeChangeInterval % 3;

        if (newZombieTypeIndex != zombieTypeIndex)
        {
            zombieTypeIndex = newZombieTypeIndex;
            currentZombiesPerWave = initialZombiePerWave;
        }
        else
        {
            // Double the number of zombies for the same type
            currentZombiesPerWave *= 2;
        }

        GlobalReferences.Instance.waveNumber = currentWave;
        currentWaveUI.text = "Wave: " + currentWave.ToString();
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < currentZombiesPerWave; i++)
        {

            GameObject zombiePrefabToSpawn = GetZombiePrefabType();

            //Generate a random offset within a specified range
            Vector3 spawnOffset = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
            Vector3 spawnPosition = transform.position + spawnOffset;

            //Instantiate the zombie
            var zombie = Instantiate(zombiePrefabToSpawn, spawnPosition, Quaternion.identity);

            //Get enemy script
            Enemy enemyScript = zombie.GetComponent<Enemy>();

            //Track this zombie
            currentZombiesAlive.Add(enemyScript);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private GameObject GetZombiePrefabType()
    {
        switch (zombieTypeIndex)
        {
            case 0: // Default zombie
                return zombiePrefab;
            case 1: // Fast zombie
                return fastZombiePrefab;
            case 2: // Strong zombie
                return strongZombiePrefab;
            default:
                return zombiePrefab;
        }
    }

    private void Update()
    {
        //Get all dead zombies
        List<Enemy> zombiesToRemove = new List<Enemy>();
        foreach (Enemy zombie in currentZombiesAlive)
        {
            if (zombie.isDead)
            {
                zombiesToRemove.Add(zombie);
            }
        }

        //Remove all dead zombies
        foreach (Enemy zombie in zombiesToRemove)
        {
            currentZombiesAlive.Remove(zombie);
        }

        zombiesToRemove.Clear();

        //start cooldown when all zombies are dead
        if (currentZombiesAlive.Count == 0 && inCooldown == false)
        {
            //Start cooldown for the next wave
            StartCoroutine(WaveCooldown());
        }

        //run the cooldown counter
        if (inCooldown)
        {
            cooldownCounter -= Time.deltaTime;
        }
        else
        {
            //reset the counter
            cooldownCounter = waveCooldown;
        }

        cooldownCounterUI.text = cooldownCounter.ToString("F0");
    }

    private IEnumerator WaveCooldown()
    {
        inCooldown = true;
        waveOverUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(waveCooldown);

        inCooldown = false;
        waveOverUI.gameObject.SetActive(false);
        //currentZombiesPerWave *= 2;
        StartNextWave();
    }
}