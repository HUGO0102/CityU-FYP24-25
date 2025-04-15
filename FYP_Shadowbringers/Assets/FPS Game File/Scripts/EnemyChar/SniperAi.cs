using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SniperAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;
    public GameObject TargetLooker;
    private MeshRenderer meshRenderer;
    public Animator animator;

    // Animation bool
    bool isIdle;
    bool isChase;
    public bool isAttack;
    bool isHit;
    bool dead;
    bool isEquiping;
    bool Equiped;
    bool isUnEquiping;

    public GameObject DestoryObj;

    // Stats
    public int health;

    // Check for Ground/Obstacles
    public LayerMask whatIsGround, whatIsPlayer;

    // Patroling
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;

    // Attack Player
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    bool hited;

    // States
    public bool isDead;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private TargetLooker targetLooker;
    private Vector3 targetPostition;

    // VFX
    [Header("VFX")]
    public ParticleSystem spark;
    public ParticleSystem onHitVFX;
    bool vfxIsCreated = false;

    private ParticleSystem onHitVFXInstance;

    [Header("Prefab Refrences")]
    public GameObject muzzleFlashPrefab;

    [Header("Location Refrences")]
    [SerializeField] private Transform barrelLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")][SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")][SerializeField] private float shotPower = 500f;

    // Sniper Attack Settings
    [Header("Sniper Attack Settings")]
    [SerializeField] private float shootInterval = 3f; // 射擊間隔（秒，瞄準時間）
    [SerializeField] private float initialDelay = 2f; // 每次射擊前的等待時間（秒）
    private Coroutine shootCoroutine;
    private float lastShootTime = 0f; // 記錄上次射擊時間

    // 射擊線設置
    [Header("Sniper Laser Settings")]
    [SerializeField] private LineRenderer laserLine; // 射擊線的 LineRenderer 組件
    [SerializeField] private float laserMaxWidth = 0.5f; // 射擊線的最大寬度
    [SerializeField] private float laserMinWidth = 0.1f; // 射擊線的最小寬度
    [SerializeField] private float laserTransitionTime = 3f; // 射擊線從粗到細的過渡時間（與 shootInterval 一致）
    [SerializeField] private Color laserStartColor = Color.red; // 射擊線的起始顏色
    [SerializeField] private Color laserEndColor = Color.red; // 射擊線的結束顏色
    private Coroutine laserCoroutine; // 用於控制射擊線動畫的協程

    // SFX
    [Header("SoundFX")]
    public AudioSource gunAudioSource;
    public AudioSource enemyAudioSource;
    public AudioClip fireSound;
    public AudioClip[] enemyAudioClip;
    [Range(0.1f, 0.5f)]
    public float volumeChangeMultiplier = 0.2f;
    [Range(0.1f, 0.5f)]
    public float pitchChangeMultiplier = 0.2f;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (targetLooker == null)
            targetLooker = GetComponentInChildren<TargetLooker>();

        // 初始化 LineRenderer
        if (laserLine == null)
        {
            laserLine = gameObject.AddComponent<LineRenderer>();
        }
        laserLine.positionCount = 2; // 設置為 2 個點（起始點和終點）
        laserLine.startColor = laserStartColor;
        laserLine.endColor = laserEndColor;
        laserLine.startWidth = 0f; // 初始寬度為 0（隱藏）
        laserLine.endWidth = 0f;
        laserLine.enabled = false; // 初始隱藏
    }

    private void Update()
    {
        SwitchAnimation();

        if (!isDead)
        {
            // Check if Player in sightrange
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

            // Check if Player in attackrange
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            targetPostition = new Vector3(player.position.x, this.transform.position.y, player.position.z);

            if (!playerInSightRange && !playerInAttackRange)
            {
                isIdle = false;
                isAttack = false;
                isChase = true;
                Patroling();
                ResetAttack();
            }
            else if (playerInSightRange && !playerInAttackRange)
            {
                isIdle = false;
                isAttack = false;
                isChase = true;
                ChasePlayer();
                ResetAttack();
            }
            else if (playerInAttackRange && playerInSightRange)
            {
                isChase = false;
                isIdle = false;
                isAttack = true;
                AttackPlayer();
            }
        }
    }

    void SwitchAnimation()
    {
        animator.SetBool("Patrol", isIdle);
        animator.SetBool("Chase", isChase);
        animator.SetBool("Attack", isAttack);
        animator.SetBool("Hit", isHit);
    }

    private void Patroling()
    {
        if (isDead) return;

        agent.speed = 2f;

        if (targetLooker != null)
            TargetLooker.GetComponent<TargetLooker>().targetTrans = null;

        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

        isAttack = false;
    }

    private void SearchWalkPoint()
    {
        if (targetLooker != null)
            TargetLooker.GetComponent<TargetLooker>().targetTrans = null;

        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (isDead) return;

        agent.speed = 4f;

        if (targetLooker != null)
            TargetLooker.GetComponent<TargetLooker>().targetTrans = player;

        agent.SetDestination(player.position);

        isAttack = false;

        Debug.Log("Sniper is chasing player!");
    }

    private void AttackPlayer()
    {
        if (isDead) return;

        agent.speed = 2f;

        if (targetLooker != null)
            TargetLooker.GetComponent<TargetLooker>().targetTrans = player;

        agent.SetDestination(transform.position);

        transform.LookAt(targetPostition);

        if (!alreadyAttacked)
        {
            alreadyAttacked = true;

            if (shootCoroutine == null)
            {
                shootCoroutine = StartCoroutine(ShootAtIntervals());
            }
        }
    }

    private void ResetAttack()
    {
        if (isDead) return;

        alreadyAttacked = false;

        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
            Debug.Log("Sniper stopped shooting coroutine.");
        }

        if (laserCoroutine != null)
        {
            StopCoroutine(laserCoroutine);
            laserCoroutine = null;
        }

        // 隱藏射擊線
        laserLine.enabled = false;
    }

    private IEnumerator ShootAtIntervals()
    {
        while (true)
        {
            if (isDead) // 檢查死亡狀態
            {
                laserLine.enabled = false; // 隱藏射擊線
                yield break; // 立即退出協程
            }

            if (isAttack)
            {
                // 等待 2 秒（每次射擊前的等待）
                yield return new WaitForSeconds(initialDelay);

                if (isDead) // 再次檢查死亡狀態
                {
                    laserLine.enabled = false;
                    yield break;
                }

                // 顯示射擊線並開始動畫
                if (laserCoroutine != null)
                {
                    StopCoroutine(laserCoroutine);
                }
                laserCoroutine = StartCoroutine(AnimateLaserLine());

                // 等待射擊間隔（3 秒，瞄準時間）
                yield return new WaitForSeconds(shootInterval);

                if (isDead) // 再次檢查死亡狀態
                {
                    laserLine.enabled = false;
                    yield break;
                }

                // 射擊
                Shoot();

                // 射擊後隱藏射擊線
                laserLine.enabled = false;
            }
            else
            {
                // 如果不在 Attack 狀態，確保射擊線隱藏
                laserLine.enabled = false;
                yield return null;
            }
        }
    }

    private IEnumerator AnimateLaserLine()
    {
        laserLine.enabled = true; // 顯示射擊線

        float elapsedTime = 0f;

        while (elapsedTime < laserTransitionTime)
        {
            if (!isAttack || isDead) // 檢查 Attack 狀態和死亡狀態
            {
                laserLine.enabled = false;
                yield break; // 如果離開 Attack 狀態或死亡，停止動畫
            }

            // 更新射擊線的位置
            laserLine.SetPosition(0, barrelLocation.position); // 起始點：槍口
            laserLine.SetPosition(1, player.position); // 終點：玩家位置

            // 從粗到細過渡
            float t = elapsedTime / laserTransitionTime;
            float currentWidth = Mathf.Lerp(laserMaxWidth, laserMinWidth, t);
            laserLine.startWidth = currentWidth;
            laserLine.endWidth = currentWidth;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 確保最後寬度為最小值
        laserLine.startWidth = laserMinWidth;
        laserLine.endWidth = laserMinWidth;
    }

    public void SetHitedtoFalse()
    {
        hited = false;
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        hited = true;

        AudioClip ramdomSFX = enemyAudioClip[Random.Range(0, 3)];
        enemyAudioSource.volume = Random.Range(1 - volumeChangeMultiplier, 1);
        enemyAudioSource.pitch = Random.Range(1 - pitchChangeMultiplier, 1 + pitchChangeMultiplier);
        enemyAudioSource.PlayOneShot(ramdomSFX);

        if (animator != null)
        {
            if (!vfxIsCreated)
            {
                if (onHitVFX != null)
                {
                    Instantiate(onHitVFX, new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), transform.rotation);
                }
            }

            animator.SetTrigger("isHited");
        }

        health -= damage;
        Debug.Log("Enemy Health: " + health);
        if (health <= 0)
        {
            isDead = true;

            // 立即停止射擊行為
            ResetAttack();

            if (animator != null)
            {
                DestroyAnimation();
            }
        }
    }

    private void DestroyAnimation()
    {
        gameObject.GetComponent<NavMeshAgent>().enabled = false;
        animator.Play("Dead");

        enemyAudioSource.PlayOneShot(enemyAudioClip[4]);

        if (!vfxIsCreated)
        {
            Instantiate(spark, new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), transform.rotation);
            vfxIsCreated = true;
        }

        Invoke("DestoryObject", 10);
    }

    public void DestoryObject()
    {
        Destroy(DestoryObj);
    }

    public void Shoot()
    {
        if (!isAttack || isDead) // 檢查 Attack 狀態和死亡狀態
        {
            Debug.Log("Sniper cannot shoot while chasing or dead!");
            return;
        }

        // 總週期為 initialDelay + shootInterval（2 秒 + 3 秒 = 5 秒）
        float totalCycleTime = initialDelay + shootInterval;
        if (Time.time - lastShootTime < totalCycleTime)
        {
            Debug.Log("Sniper is on cooldown!");
            return;
        }

        lastShootTime = Time.time;

        Debug.Log("Sniper shoot");
        gunAudioSource.PlayOneShot(fireSound);

        if (muzzleFlashPrefab)
        {
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
            Destroy(tempFlash, destroyTimer);
        }

        // 從 BulletPoolManager 獲取子彈
        GameObject bullet = BulletPoolManager.Instance.GetEnemyBullet();
        bullet.SetActive(true);
        bullet.transform.position = barrelLocation.position;
        bullet.transform.rotation = barrelLocation.rotation;

        // 設置子彈標籤為 Sniper_Bullet
        bullet.tag = "SniperBullets";

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(barrelLocation.forward * shotPower);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}