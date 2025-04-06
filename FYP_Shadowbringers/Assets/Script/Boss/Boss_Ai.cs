
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
public class Boss_Ai : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;
    public GameObject TargetLooker;
    private MeshRenderer meshRenderer;
    public Animator animator;


    //Animation bool
    bool isIdle;
    bool isWalking;
    bool isAttack;
    bool isHit;
    bool dead;

    public GameObject DestoryObj;

    //Stats
    public int health;

    //Check for Ground/Obstacles
    public LayerMask whatIsGround, whatIsPlayer;

    //Patroling
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;

    [Header("Attack")]
    //Attack Player
    public int timeBetweenAttacks = 2;
    public bool alreadyAttacked;
    public int fire_Density = 10;
    public float fire_Gap = 0.1f;
    private int Attack_SkillsNumber = 0;
    private int RanNum = 0;


    // 子彈對象池
    private Queue<GameObject> bulletPool = new Queue<GameObject>();
    [SerializeField] private int bulletPoolSize = 20; // 對象池大小



    [Header("States")] 
    public bool isDead;
    public bool hited;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;


    private TargetLooker targetLooker;
    private Vector3 targetPostition;

    //VFX
    [Header("VFX")]
    public ParticleSystem spark;
    public ParticleSystem onHitVFX;
    bool vfxIsCreated = false;

    private ParticleSystem onHitVFXInstance;


    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location Refrences")]
    [SerializeField] private Transform barrelLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")][SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")][SerializeField] private float shotPower = 500f;

    //SFX
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


    }

    private void Start()
    {
        // 初始化對象池
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }


    private void Update()
    {
        SwitchAnimation();

        if (!isDead)
        {
            //Check if Player in sightrange
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

            //Check if Player in attackrange
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            targetPostition = new Vector3(player.position.x, this.transform.position.y, player.position.z);

            if (!playerInSightRange && !playerInAttackRange)
            {
                isIdle = true;
                isWalking = false;
                Idleing();
            }
            if (playerInSightRange && !playerInAttackRange)
            {

                isIdle = false;
                isWalking = true;
                ChasePlayer();
            }
            if (playerInAttackRange && playerInSightRange)
            {
                isIdle = true;
                isWalking = false;              
                AttackPlayer();
            }
        }
    }

    void SwitchAnimation()
    {
        animator.SetBool("Idle", isIdle);
        animator.SetBool("Walking", isWalking);
        animator.SetBool("Attack", isAttack);
        animator.SetBool("Hit", isHit);
    }

    private void Idleing()
    {
        if (isDead) return;

        agent.speed = 2f;

        if (targetLooker != null)
            TargetLooker.GetComponent<TargetLooker>().targetTrans = null;

        agent.SetDestination(transform.position);

        //Calculates DistanceToWalkPoint
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

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

        agent.speed = 5f;

        if (targetLooker != null)
            TargetLooker.GetComponent<TargetLooker>().targetTrans = player;

        agent.SetDestination(player.position);
    }


    private void AttackPlayer()
    {
        if (isDead) return;

        agent.speed = 2f;

        if (targetLooker != null)
            TargetLooker.GetComponent<TargetLooker>().targetTrans = player;

        // 使槍口也朝向玩家
        barrelLocation.LookAt(player.position);

        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        

        if (!alreadyAttacked)
        {
            while (Attack_SkillsNumber == RanNum)
            {
                Attack_SkillsNumber = Random.Range(0, 2);
            }
            RanNum = Attack_SkillsNumber;

            switch (Attack_SkillsNumber)
            {
                case 1:
                    {
                        GunAttack();
                        print("Attack Case 1");
                        break;
                    }
                case 0:
                    {
                        GunAttack();
                        print("Attack Case 0");
                        break;
                    }
                default:
                    {
                        break;
                    }

            }
            alreadyAttacked = true;
        }
    }


    private void GunAttack()
    {
        isAttack = true; // 觸發攻擊動畫
        StartCoroutine(FireBurst());
    }

    private IEnumerator FireBurst()
    {
        int i = 0;
        while (i <= fire_Density)
        {
            Shoot();
            i++;
            yield return new WaitForSeconds(fire_Gap); // 每發子彈間隔 0.1 秒
        }
        isAttack = false; // 停止攻擊動畫
        Invoke("ResetAttack", timeBetweenAttacks); // 射擊完成後停頓 2 秒
    }


    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    //=================================================================================================================================================================================


    public void TakeDamage(int damage)
    {
        hited = true;

        //SFX
        AudioClip ramdomSFX = enemyAudioClip[Random.Range(0, 3)];
        enemyAudioSource.volume = Random.Range(1 - volumeChangeMultiplier, 1);
        enemyAudioSource.pitch = Random.Range(1 - pitchChangeMultiplier, 1 + pitchChangeMultiplier);
        enemyAudioSource.PlayOneShot(ramdomSFX);


        if (animator != null)
        {
            //VFX
            if (!vfxIsCreated)
            {
                SpawnOnHitVFX();
            }

            animator.SetTrigger("isHited");
        }


        health -= damage;
        Debug.Log("Enemy Health" + health);
        if (health <= 0)
        {
            isDead = true;

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

        //VFX
        if (!vfxIsCreated)
        {
            Instantiate(spark, new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), transform.rotation);
            vfxIsCreated = true;
        }

        //Delay 10sec
        Invoke("DestoryObject", 10);
    }

    public void DestoryObject()
    {
        Destroy(DestoryObj);
    }

    private void SpawnOnHitVFX()
    {
        onHitVFXInstance = Instantiate(onHitVFX, new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), transform.rotation);
    }

    //=================================================================================================================================================================================


    public void Shoot()
    {
        gunAudioSource.PlayOneShot(fireSound);
        gunAudioSource.pitch = Random.Range(1 - pitchChangeMultiplier, 1 + pitchChangeMultiplier);

        if (muzzleFlashPrefab)
        {
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
            Destroy(tempFlash, destroyTimer);
        }

        // 從對象池中獲取子彈
        GameObject bullet = BulletPoolManager.Instance.GetEnemyBullet();
        bullet.SetActive(true);
        bullet.transform.position = barrelLocation.position;

        // 計算朝向玩家的方向
        Vector3 directionToPlayer = (player.position - barrelLocation.position).normalized;
        bullet.transform.rotation = Quaternion.LookRotation(directionToPlayer);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(barrelLocation.forward * shotPower);
    }

    private IEnumerator ReturnBulletToPool(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }

    //=================================================================================================================================================================================

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

}
