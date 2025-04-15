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
    [SerializeField] private float shootInterval = 3f; // �g�����j�]��A�˷Ǯɶ��^
    [SerializeField] private float initialDelay = 2f; // �C���g���e�����ݮɶ��]��^
    private Coroutine shootCoroutine;
    private float lastShootTime = 0f; // �O���W���g���ɶ�

    // �g���u�]�m
    [Header("Sniper Laser Settings")]
    [SerializeField] private LineRenderer laserLine; // �g���u�� LineRenderer �ե�
    [SerializeField] private float laserMaxWidth = 0.5f; // �g���u���̤j�e��
    [SerializeField] private float laserMinWidth = 0.1f; // �g���u���̤p�e��
    [SerializeField] private float laserTransitionTime = 3f; // �g���u�q�ʨ�Ӫ��L��ɶ��]�P shootInterval �@�P�^
    [SerializeField] private Color laserStartColor = Color.red; // �g���u���_�l�C��
    [SerializeField] private Color laserEndColor = Color.red; // �g���u�������C��
    private Coroutine laserCoroutine; // �Ω󱱨�g���u�ʵe����{

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

        // ��l�� LineRenderer
        if (laserLine == null)
        {
            laserLine = gameObject.AddComponent<LineRenderer>();
        }
        laserLine.positionCount = 2; // �]�m�� 2 ���I�]�_�l�I�M���I�^
        laserLine.startColor = laserStartColor;
        laserLine.endColor = laserEndColor;
        laserLine.startWidth = 0f; // ��l�e�׬� 0�]���á^
        laserLine.endWidth = 0f;
        laserLine.enabled = false; // ��l����
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

        // ���îg���u
        laserLine.enabled = false;
    }

    private IEnumerator ShootAtIntervals()
    {
        while (true)
        {
            if (isDead) // �ˬd���`���A
            {
                laserLine.enabled = false; // ���îg���u
                yield break; // �ߧY�h�X��{
            }

            if (isAttack)
            {
                // ���� 2 ��]�C���g���e�����ݡ^
                yield return new WaitForSeconds(initialDelay);

                if (isDead) // �A���ˬd���`���A
                {
                    laserLine.enabled = false;
                    yield break;
                }

                // ��ܮg���u�ö}�l�ʵe
                if (laserCoroutine != null)
                {
                    StopCoroutine(laserCoroutine);
                }
                laserCoroutine = StartCoroutine(AnimateLaserLine());

                // ���ݮg�����j�]3 ��A�˷Ǯɶ��^
                yield return new WaitForSeconds(shootInterval);

                if (isDead) // �A���ˬd���`���A
                {
                    laserLine.enabled = false;
                    yield break;
                }

                // �g��
                Shoot();

                // �g�������îg���u
                laserLine.enabled = false;
            }
            else
            {
                // �p�G���b Attack ���A�A�T�O�g���u����
                laserLine.enabled = false;
                yield return null;
            }
        }
    }

    private IEnumerator AnimateLaserLine()
    {
        laserLine.enabled = true; // ��ܮg���u

        float elapsedTime = 0f;

        while (elapsedTime < laserTransitionTime)
        {
            if (!isAttack || isDead) // �ˬd Attack ���A�M���`���A
            {
                laserLine.enabled = false;
                yield break; // �p�G���} Attack ���A�Φ��`�A����ʵe
            }

            // ��s�g���u����m
            laserLine.SetPosition(0, barrelLocation.position); // �_�l�I�G�j�f
            laserLine.SetPosition(1, player.position); // ���I�G���a��m

            // �q�ʨ�ӹL��
            float t = elapsedTime / laserTransitionTime;
            float currentWidth = Mathf.Lerp(laserMaxWidth, laserMinWidth, t);
            laserLine.startWidth = currentWidth;
            laserLine.endWidth = currentWidth;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �T�O�̫�e�׬��̤p��
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

            // �ߧY����g���欰
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
        if (!isAttack || isDead) // �ˬd Attack ���A�M���`���A
        {
            Debug.Log("Sniper cannot shoot while chasing or dead!");
            return;
        }

        // �`�g���� initialDelay + shootInterval�]2 �� + 3 �� = 5 ��^
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

        // �q BulletPoolManager ����l�u
        GameObject bullet = BulletPoolManager.Instance.GetEnemyBullet();
        bullet.SetActive(true);
        bullet.transform.position = barrelLocation.position;
        bullet.transform.rotation = barrelLocation.rotation;

        // �]�m�l�u���Ҭ� Sniper_Bullet
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