using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TreeEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieSpawnController : MonoBehaviour
{
    public int initialZombiePerWave = 5;
    public int currentZombiesPerWave;

    public float spawnDelay = 0.5f; //Delay between spawning each zombie in a wave

    public int currentWave = 0;
    public float waveCooldown = 10.0f; // Cooldown between waves

    public bool inCooldown;
    public float cooldownCounter = 0; // use for testing

    public List<Enemy> currentZombiesAlive;

    public GameObject zombiePrefab;

    public TextMeshProUGUI waveOverUI;
    public TextMeshProUGUI cooldownCounterUI;

    public TextMeshProUGUI currentWaveUI;
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

        GlobalReferences.Instance.waveNumber = currentWave;
        currentWaveUI.text = "Wave: " + currentWave.ToString();
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < currentZombiesPerWave; i++)
        {
            //Generate a random offset within a specified range
            Vector3 spawnOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            Vector3 spawnPosition = transform.position + spawnOffset;

            //Instantiate the zombie
            var zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);

            //Get enemy script
            Enemy enemyScript = zombie.GetComponent<Enemy>();

            //Track this zombie
            currentZombiesAlive.Add(enemyScript);

            yield return new WaitForSeconds(spawnDelay);
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
        currentZombiesPerWave += 2;
        StartNextWave();
    }
}