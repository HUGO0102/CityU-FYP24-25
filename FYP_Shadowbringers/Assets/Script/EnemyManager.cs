using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [Header("Spawning Settings")]
    [Tooltip("List of enemy prefabs to spawn randomly")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    [Tooltip("Maximum number of enemies allowed in the scene")]
    public int maxSpawnEnemies = 10;

    public int maxEnemyOnArea;

    [Tooltip("Time interval between spawns (in seconds)")]
    public float spawnInterval = 5f;

    [Header("TriggerController")]
    public TriggerController triggerController;
    public int TriggerIndex;
    public bool canSpawn;

    [Header("Spawn Points")]
    [Tooltip("List of spawn points where enemies can spawn")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Alife Enemys")]
    public List<GameObject> activeEnemies = new List<GameObject>();

    private void Start()
    {
        // Start the spawning coroutine
        StartCoroutine(SpawnEnemiesCoroutine());
    }


    private void Update()
    {
        if (TriggerIndex == 1)
        {
            if (triggerController.canSpawn1 == true)
                canSpawn = true;
        }
        else if(TriggerIndex == 2)
        {
            if (triggerController.canSpawn2 == true)
                canSpawn = true;
        }
        else if (TriggerIndex == 5)
        {
            if (triggerController.canSpawn4 == true)
                canSpawn = true;
        }

        CheckForDeadEnemies();
    }

    /// <summary>
    /// For spawn enemys when the enemys is less than max enemys
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEnemiesCoroutine()
    {
        int totalSpawnedEnemies = 0;

        while (true)
        {
            // Wait for the spawn interval
            yield return new WaitForSeconds(spawnInterval);

            if (canSpawn)
            {
                // Check if we can spawn more enemies
                if (totalSpawnedEnemies < maxSpawnEnemies && activeEnemies.Count < maxEnemyOnArea)
                {
                    // Spawn an enemy and increment the total spawn counter
                    SpawnEnemy();
                    totalSpawnedEnemies++;
                }
            }
            if (totalSpawnedEnemies == maxSpawnEnemies && activeEnemies.Count == 0)
            {
                if (TriggerIndex == 1)
                {
                    triggerController.Collider01.gameObject.SetActive(false);
                    triggerController.Collider02.gameObject.SetActive(false);
                }
                else if (TriggerIndex == 2)
                {
                    triggerController.Collider03.gameObject.SetActive(false);
                    triggerController.Collider04.gameObject.SetActive(false);
                }
                else if (TriggerIndex == 5)
                {
                    triggerController.Collider05.gameObject.SetActive(false);
                    triggerController.Collider06.gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Spawn enemy function
    /// </summary>
    private void SpawnEnemy()
    {
        if (spawnPoints.Count == 0 || enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("No spawn points or enemy prefabs assigned!");
            return;
        }

        // Choose a random spawn point
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // Choose a random enemy prefab
        GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        GameObject newEnemy = Instantiate(randomEnemyPrefab, randomSpawnPoint.position, Quaternion.identity);

        activeEnemies.Add(newEnemy);

        /*Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.OnDeath += () => RemoveEnemyFromList(newEnemy);
        }*/
    }

    /// <summary>
    /// Remove the Enemy if they're dead
    /// </summary>
    /// <param name="enemy"></param>
    private void RemoveEnemyFromList(GameObject enemy)
    {
        // Remove the enemy from the active enemies list
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    private void CheckForDeadEnemies()
    {
        // Iterate through the list in reverse to safely remove elements
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = activeEnemies[i];

            // Check if the enemy has a MeleeAi script and is dead
            MeleeAi meleeAi = enemy.GetComponent<MeleeAi>();
            if (meleeAi != null && meleeAi.isDead)
            {
                RemoveEnemyFromList(enemy);
                continue;
            }

            // Check if the enemy has a SniperAi script and is dead
            SniperAi sniperAi = enemy.GetComponent<SniperAi>();
            if (sniperAi != null && sniperAi.isDead)
            {
                RemoveEnemyFromList(enemy);
                continue;
            }

            // Check if the enemy has a ShootingAi script and is dead
            ShootingAi shootingAi = enemy.GetComponent<ShootingAi>();
            if (shootingAi != null && shootingAi.isDead)
            {
                RemoveEnemyFromList(enemy);
                continue;
            }
        }
    }

    /// <summary>
    /// Draws debug visuals for the spawn points in the editor.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw a sphere at each spawn point
        Gizmos.color = Color.red;
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
            }
        }
    }
}