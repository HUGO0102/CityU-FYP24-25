
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


    [Header("Prefab Refrences")]
    public GameObject muzzleFlashPrefab;

    //Barrel
    [Header("Location Refrences")]
    [SerializeField] private Transform rBarrelLocation;
    [SerializeField] private Transform lBarrelLocation;
    [Header("Barrel Spinners")]
    [SerializeField] private BarrelSpinner rightBarrelSpinner;
    [SerializeField] private BarrelSpinner leftBarrelSpinner;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")][SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")][SerializeField] private float shotPower = 500f;

    //SFX
    [Header("SoundFX")]
    public AudioSource gunAudioSource;
    public AudioSource enemyAudioSource;
    public AudioClip fireSound;
    public AudioClip[] enemyAudioClip;

    public AudioSource rightBarrelAudioSource;
    public AudioSource leftBarrelAudioSource;
    public AudioClip barrelSpinSound;

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

  


    private void Update()
    {
        SwitchAnimation();

        // �ھڦ�q�վ�����Ѽ�
        if (health <= (health/2))
        {
            fire_Density = 15; // �W�[�g���K��
            fire_Gap = 0.08f; // ��֮g�����j
        }
        else
        {
            fire_Density = 10;
            fire_Gap = 0.1f;
        }



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

        // �Ϻj�f�¦V���a
        rBarrelLocation.LookAt(player.position);
        lBarrelLocation.LookAt(player.position);

        // �T�O�ĤH������
        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {
            while (Attack_SkillsNumber == RanNum)
            {
                Attack_SkillsNumber = Random.Range(0, 3); // �W�[�� 3 �ؼҦ�
            }
            RanNum = Attack_SkillsNumber;

            switch (Attack_SkillsNumber)
            {
                case 2:
                    GunAttackBoth(); // �P�ɨϥΥ��k�j�f
                    print("Both Guns Attack");
                    break;
                case 1:
                    GunAttackRight(); // �ȨϥΥk�j�f
                    print("Right Gun Attack");
                    break;
                case 0:
                    GunAttackLeft(); // �ȨϥΥ��j�f
                    print("Left Gun Attack");
                    break;
                default:
                    break;
            }
            alreadyAttacked = true;
        }
    }

    private void GunAttackRight()
    {
        isAttack = true;
        StartCoroutine(FireBurst(rBarrelLocation));
    }

    private void GunAttackLeft()
    {
        isAttack = true;
        StartCoroutine(FireBurst(lBarrelLocation));
    }

    private void GunAttackBoth()
    {
        isAttack = true;
        StartCoroutine(FireBurstBoth());
    }

    private IEnumerator FireBurst(Transform barrel)
    {
        int i = 0;
        BarrelSpinner selectedSpinner = (barrel == rBarrelLocation) ? rightBarrelSpinner : leftBarrelSpinner;
        selectedSpinner.StartSpinning(); // �}�l����

        AudioSource selectedAudioSource = (barrel == rBarrelLocation) ? rightBarrelAudioSource : leftBarrelAudioSource;
        selectedAudioSource.clip = barrelSpinSound;
        selectedAudioSource.loop = true;
        selectedAudioSource.Play(); // ������୵��

        while (i <= fire_Density)
        {
            Shoot(barrel);
            i++;
            yield return new WaitForSeconds(fire_Gap); // �C�o�l�u���j 0.1 ��
        }
        selectedSpinner.StopSpinning(); // �������
        selectedAudioSource.Stop(); // ������୵��
        isAttack = false; // ��������ʵe
        Invoke("ResetAttack", timeBetweenAttacks); // �g�������ᰱ�y 2 ��
    }

    private IEnumerator FireBurstBoth()
    {
        int i = 0;
        bool useRightBarrel = true;
        while (i <= fire_Density)
        {
            Transform selectedBarrel = useRightBarrel ? rBarrelLocation : lBarrelLocation;
            BarrelSpinner selectedSpinner = useRightBarrel ? rightBarrelSpinner : leftBarrelSpinner;
            selectedSpinner.StartSpinning(); // �}�l����
            Shoot(selectedBarrel);
            i++;
            useRightBarrel = !useRightBarrel; // ����ϥΥ��k�j�f
            yield return new WaitForSeconds(fire_Gap);
        }
        rightBarrelSpinner.StopSpinning(); // �������
        leftBarrelSpinner.StopSpinning();
        isAttack = false;
        Invoke("ResetAttack", timeBetweenAttacks + 2);
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
                if (onHitVFX != null)
                {
                    Instantiate(onHitVFX, new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), transform.rotation);
                }
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


    //=================================================================================================================================================================================


    public void Shoot(Transform barrel)
    {
        gunAudioSource.PlayOneShot(fireSound);
        gunAudioSource.pitch = Random.Range(1 - pitchChangeMultiplier, 1 + pitchChangeMultiplier);

        if (muzzleFlashPrefab)
        {
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrel.position, barrel.rotation);
            Destroy(tempFlash, destroyTimer);
        }

        // �q��H��������l�u
        GameObject bullet = BulletPoolManager.Instance.GetEnemyBullet();
        bullet.SetActive(true);
        bullet.transform.position = barrel.position;

        // �p��¦V���a����V
        Vector3 directionToPlayer = (player.position - barrel.position).normalized;
        bullet.transform.rotation = Quaternion.LookRotation(directionToPlayer);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(directionToPlayer * shotPower); // �ϥέp��X����V�A�Ӥ��O barrel.forward
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
