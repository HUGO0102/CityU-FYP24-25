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
    [SerializeField] private GameObject playerBulletPrefab; // 普通子彈預製體
    [SerializeField] private GameObject burstBulletPrefab; // HitOnBeat 子彈預製體
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

        // 初始化敵人子彈池
        for (int i = 0; i < enemyPoolSize; i++)
        {
            GameObject bullet = Instantiate(enemyBulletPrefab);
            bullet.SetActive(false);
            enemyBulletPool.Enqueue(bullet);
        }

        // 初始化玩家子彈池（初始使用普通子彈）
        for (int i = 0; i < playerPoolSize; i++)
        {
            GameObject bullet = Instantiate(playerBulletPrefab);
            bullet.SetActive(false);
            bullet.tag = "PlayerBullets"; // 初始標籤
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
        newBullet.SetActive(false);
        enemyBulletPool.Enqueue(newBullet);
        return newBullet;
    }

    public GameObject GetPlayerBullet(bool hitOnBeat = false)
    {
        while (playerBulletPool.Count > 0)
        {
            GameObject bullet = playerBulletPool.Dequeue();
            if (bullet != null)
            {
                bullet.tag = hitOnBeat ? "PlayerBrustBullets" : "PlayerBullets";
                return bullet;
            }
        }

        // 如果池中無可用子彈，根據 hitOnBeat 創建新子彈
        GameObject newBullet = Instantiate(hitOnBeat ? burstBulletPrefab : playerBulletPrefab);
        newBullet.tag = hitOnBeat ? "PlayerBrustBullets" : "PlayerBullets";
        newBullet.SetActive(false);
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
            bullet.tag = "PlayerBullets"; // 重置標籤為普通子彈
            if (!playerBulletPool.Contains(bullet))
            {
                playerBulletPool.Enqueue(bullet);
            }
        }
    }
}