using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [Header("Spawning Settings")]
    [Tooltip("Enemy prefab to spawn")]
    public GameObject enemyPrefab;

    [Tooltip("Maximum number of enemies allowed in the scene")]
    public int maxEnemies = 10;

    [Tooltip("Spawn radius around the manager")]
    public float spawnRadius = 10f;

    [Tooltip("Time interval between spawns (in seconds)")]
    public float spawnInterval = 5f;

    [Tooltip("Height offset for enemy spawn position")]
    public float spawnHeight = 1f;

    [Tooltip("Tag used to identify enemies in the scene")]
    public string enemyTag = "Enemy";

    [Header("Spawner Center")]
    public Transform SpawnCenter;

    private List<GameObject> activeEnemies = new List<GameObject>();

    public static EnemyManager Instance;

    private void Awake()
    {
        MakeInstance();
    }
    void MakeInstance()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Detect and track enemys that in the scene
        FindExistingEnemies();

        // Start the spawning coroutine
        StartCoroutine(SpawnEnemiesCoroutine());
    }

    /// <summary>
    /// Look for all enemy in the scene
    /// </summary>
    private void FindExistingEnemies()
    {
        // Find all Enemy with Tag in scene
        GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag(enemyTag);

        // Add enemy in the list
        foreach (GameObject enemy in existingEnemies)
        {
            if (!activeEnemies.Contains(enemy))
            {
                activeEnemies.Add(enemy);

                // Subscribe to OnDeath if the enemy has an Enemy script
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.OnDeath += () => RemoveEnemyFromList(enemy);
                }
            }
        }
    }

    /// <summary>
    /// For spawn enemys when the enemys is less than max enemys
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEnemiesCoroutine()
    {
        while (true)
        {
            // Wait for the spawn interval
            yield return new WaitForSeconds(spawnInterval);

            // Spawn enemies 
            if (activeEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
            }
        }
    }

    /// <summary>
    /// Spawn enemy function
    /// </summary>
    private void SpawnEnemy()
    {
        // Spawn enemy in the Radius that set in inspector
        Vector3 randomPosition = GetRandomPositionWithinRadius();

        // spawn the enemy
        GameObject newEnemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);

        // Add the enemy to the list
        activeEnemies.Add(newEnemy);

        // Handle enemy death
        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.OnDeath += () => RemoveEnemyFromList(newEnemy);
        }
    }

    /// <summary>
    /// look for spawn pos
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomPositionWithinRadius()
    {
        // Generate a random position
        Vector3 randomDirection = SpawnCenter.position + UnityEngine.Random.insideUnitSphere * spawnRadius;

        // Spawn the enemy highter, or the enemys will spawn under plane 
        randomDirection.y = spawnHeight;

        // Return the position
        return transform.position + randomDirection;
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

    /// <summary>
    /// Draw the Radius for debug, can comment the function to hide it
    /// and this function dont need to call to active
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw the spawn radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}