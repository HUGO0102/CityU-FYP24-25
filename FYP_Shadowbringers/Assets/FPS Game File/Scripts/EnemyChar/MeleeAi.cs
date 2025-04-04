
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
public class MeleeAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;
    public GameObject TargetLooker;
    private MeshRenderer meshRenderer;
    public Animator animator;


    //Animation bool
    bool isIdle;
    bool isChase;
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

    //Attack Player
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    bool hited;
    public Collider Attack_Collider;

    //States
    public bool isDead;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;


    private TargetLooker targetLooker;
    private Vector3 targetPostition;

    //VFX
    [Header("VFX")]
    public GameObject spark;
    public GameObject onHitVFX;
    bool vfxIsCreated = false;

    //SFX
    [Header("SoundFX")]
    public AudioSource enemyAudioSource;
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

            if (!playerInSightRange && !playerInAttackRange && !alreadyAttacked)
            {
                isIdle = false;
                isAttack = false;
                isChase = true;
                Patroling();
            }
            if (playerInSightRange && !playerInAttackRange && !alreadyAttacked)
            {

                isIdle = false;
                isAttack = false;
                isChase = true;
                ChasePlayer();
            }
            if (playerInAttackRange && playerInSightRange)
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
        animator.SetBool("Attacking", isAttack);
        animator.SetBool("Hit", isHit);
        animator.SetBool("Attacked", alreadyAttacked);
    }

    private void Patroling()
    {
        if (isDead) return;

        agent.speed = 2f;

        if (targetLooker != null)
            TargetLooker.GetComponent<TargetLooker>().targetTrans = null;

        if (!walkPointSet) SearchWalkPoint();

        //Calculate direction and walk to Point
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

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

        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(targetPostition);

        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
        }


    }
    private void ResetAttack()
    {
        if (isDead) return;
        alreadyAttacked = false;
    }

    public void SetHitedtoFalse()
    {
        hited = false;
        vfxIsCreated = false;
    }

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
                Instantiate(onHitVFX, new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), transform.rotation);
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

    public void DisableCollider()
    {
        Attack_Collider.enabled = false;
    }

    public void EnableCollider()
    {
        Attack_Collider.enabled = true;
    }


}
