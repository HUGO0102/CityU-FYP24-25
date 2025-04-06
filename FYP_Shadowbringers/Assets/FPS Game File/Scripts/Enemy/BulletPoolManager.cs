using UnityEngine;
using System.Collections.Generic;

public class BulletPoolManager : MonoBehaviour
{
    public static BulletPoolManager Instance { get; private set; }

    [Header("Enemy Bullet Pool")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private int enemyPoolSize = 50;
    private Queue<GameObject> enemyBulletPool = new Queue<GameObject>();

    [Header("Player Bullet Pool")]
    [SerializeField] private GameObject playerBulletPrefab;
    [SerializeField] private int playerPoolSize = 50;
    private Queue<GameObject> playerBulletPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < enemyPoolSize; i++)
        {
            GameObject bullet = Instantiate(enemyBulletPrefab);
            bullet.SetActive(false);
            enemyBulletPool.Enqueue(bullet);
        }

        for (int i = 0; i < playerPoolSize; i++)
        {
            GameObject bullet = Instantiate(playerBulletPrefab);
            bullet.SetActive(false);
            playerBulletPool.Enqueue(bullet);
        }
    }

    public GameObject GetEnemyBullet()
    {
        while (enemyBulletPool.Count > 0)
        {
            GameObject bullet = enemyBulletPool.Dequeue();
            if (bullet != null)
            {
                return bullet;
            }
        }

        GameObject newBullet = Instantiate(enemyBulletPrefab);
        enemyBulletPool.Enqueue(newBullet);
        return newBullet;
    }

    public GameObject GetPlayerBullet()
    {
        while (playerBulletPool.Count > 0)
        {
            GameObject bullet = playerBulletPool.Dequeue();
            if (bullet != null)
            {
                return bullet;
            }
        }

        GameObject newBullet = Instantiate(playerBulletPrefab);
        playerBulletPool.Enqueue(newBullet);
        return newBullet;
    }

    public void ReturnEnemyBullet(GameObject bullet)
    {
        if (bullet != null)
        {
            bullet.SetActive(false);
            if (!enemyBulletPool.Contains(bullet))
            {
                enemyBulletPool.Enqueue(bullet);
            }
        }
    }

    public void ReturnPlayerBullet(GameObject bullet)
    {
        if (bullet != null)
        {
            bullet.SetActive(false);
            if (!playerBulletPool.Contains(bullet))
            {
                playerBulletPool.Enqueue(bullet);
            }
        }
    }
}