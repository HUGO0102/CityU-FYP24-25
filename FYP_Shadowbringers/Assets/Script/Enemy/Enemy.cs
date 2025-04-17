using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;

public class Enemy : MonoBehaviour
{

    public enum EnemyType
    {
        Melee,
        Ranged
    }

    [Header("Enemy Type")]
    public EnemyType enemyType = EnemyType.Melee; // Default to Melee

    [Header("Manger")]
    public NavMeshAgent agent;

    [Header("State")]
    public float health = 5;
    private float maxhealth;

    private float startSpeed;
    private float speed;


    [Header("WalkAroundSetting")]
    private Transform patrolCenter; // Center point of the WalkAround area
    private float patrolRadius; // Radius of the WalkAround area

    [Header("Player Detecter")]
    public float meleeDetectionRadius = 10f;
    public float rangedDetectionRadius = 15f;

    [Header("Render")]
    public Renderer enemyRenderer; //Enemy's Renderer component
    public Color hitColor = Color.red; // The color to change to when hit
    public float colorChangeDuration = 0.1f; // Duration of the color change

    private Material enemyMaterial; // Material of the enemy
    private Color originalColor; // Original color of the enemy

    private Vector3 targetPosition;

    private float idleTime = 2f; // Time to wait before moving again
    private float idleTimer;

    private Transform Player;
    private CharacterController PlayerCharacterController;

    private bool isChasingPlayer;

    public Animator enemyAnim;

    public event Action OnDeath;

    [Header("Melee Attack Settings")]
    [SerializeField] private float StopMovingAttacGap = 4f;
    [SerializeField] private float RotationValue = 0f;
    [Header("Range Enemy Setting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float BulletSpeed = 20;
    [SerializeField] private float ShootingGapSec = 5f;
    private float FixShootingGapSec;

    [Header("Aiming Line Settings")]
    [SerializeField] private LineRenderer aimingLine;
    [SerializeField] private float aimingLineStartShowTime = 3f;
    [SerializeField] private Color aimingLineColor = Color.red; 
    [SerializeField] private float lineWidth = 0.02f;

    private bool isShooted;
    // Start is called before the first frame update
    void Start()
    {
        //patrolCenter = EnemyManager.Instance.SpawnCenter;
        patrolCenter = this.gameObject.transform;

        //patrolRadius = EnemyManager.Instance.spawnRadius;
        patrolRadius = 10f;

        agent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerCharacterController = Player.GetComponent<CharacterController>();

        enemyRenderer = GetComponent<Renderer>();
        //enemyMaterial = enemyRenderer.material;
        //originalColor = enemyMaterial.color;

        startSpeed = agent.speed;
        speed = startSpeed;

        FixShootingGapSec = ShootingGapSec;

        maxhealth = health;

        if (enemyType == EnemyType.Ranged)
        {
            if (aimingLine != null)
            {
                aimingLine.startColor = aimingLineColor;
                aimingLine.endColor = aimingLineColor;
                aimingLine.startWidth = lineWidth;
                aimingLine.endWidth = lineWidth;
                aimingLine.enabled = false;
            }
        }
    }

    void Update()
    {
        if (health <= 0)
        {
            Dead();
        }

        switch (enemyType)
        {
            case EnemyType.Melee:
                MeleeBehavior();
                break;

            case EnemyType.Ranged:
                RangedBehavior();
                ShootTimer();
                break;
        }    
    }

    private void MeleeBehavior()
    {
        // Check if the player is walked in the mele detection radius
        if (PlayerInDetectionRadius(meleeDetectionRadius))
        {
            StartChasingPlayer();
        }
        else
        {
            StopChasingPlayer();
        }

        // If not chasing the player
        if (!isChasingPlayer)
        {
            Patrol();
        }
        else
        {
            float distance = Vector3.Distance(Player.transform.position, transform.position);

            NavMeshAgent temp = agent;

            if (temp != null)
            {
                if (distance >= agent.stoppingDistance)
                {
                    if (enemyAnim != null)
                        enemyAnim.SetBool("isAttack", false);
                    agent.SetDestination(Player.position);
                }
                else
                {
                    //Enemy is Close to Player
                    //Can Do Enemy Attack Here
                    if (enemyAnim != null)
                        enemyAnim.SetBool("isAttack", true);
                    agent.speed = 0;
                    StartCoroutine(AttactedThanStartWalk());
                }
            }
        }
    }

    private IEnumerator AttactedThanStartWalk()
    {
        yield return new WaitForSeconds(StopMovingAttacGap);
        agent.speed = startSpeed;
    }

    private void RangedBehavior()
    {
        // Check if the player is within ranged detection radius
        if (PlayerInDetectionRadius(rangedDetectionRadius))
        {
            StartChasingPlayer();
        }

        if (isChasingPlayer)
        {
            float distance = Vector3.Distance(Player.transform.position, transform.position);

            RotateToFacePlayer();

            if (distance > rangedDetectionRadius - 5)
            {
                // Chase the player but maintain a distance
                agent.speed = startSpeed;
                agent.SetDestination(Player.position);   

                if (enemyAnim != null)
                    enemyAnim.SetBool("isRunning", true);

                if (!isShooted)
                {
                    StartShooting(); // Shoot once when the player moves out of range
                }
                //StopShooting();
            }
            else
            {
                // Stop and attack the player from a distance
                agent.speed = 0;
                agent.SetDestination(transform.position);

                if (enemyAnim != null)
                    enemyAnim.SetBool("isRunning", false);

                StartShooting();
            }
        }
        else
        {
            Patrol();
        }
    }

    private void StartShooting()
    {
        if (!isShooted)
        {
            isShooted = true;
            RangedAttack();
        }
    }

    private void StopShooting()
    {
        if (isShooted)
        {
            isShooted = false;
            ShootingGapSec = FixShootingGapSec;
        }

        if (aimingLine != null && aimingLine.enabled)
        {
            aimingLine.enabled = false;
        }
    }

    private void ShootTimer()
    {
        if (isShooted)
        {
            ShootingGapSec -= Time.deltaTime;

            // Keep updating the aiming line
            if (aimingLine != null && aimingLine.enabled)
            {
                aimingLine.SetPosition(0, projectileSpawnPoint.position);
                aimingLine.SetPosition(1, PlayerCharacterController.bounds.center);
            }

            // Enable the aiming line when the timer reaches the aiming line start time
            if (ShootingGapSec < aimingLineStartShowTime && aimingLine != null)
            {
                aimingLine.enabled = true;
                aimingLine.SetPosition(0, projectileSpawnPoint.position);
                aimingLine.SetPosition(1, PlayerCharacterController.bounds.center);
            }

            // When the shooting timer ends, reset the state
            if (ShootingGapSec <= 0)
            {
                isShooted = false; // Allow the enemy to shoot again
                ShootingGapSec = FixShootingGapSec; // Reset the cooldown

                // Disable the aiming line after shooting
                if (aimingLine != null)
                {
                    aimingLine.enabled = false;
                }
            }
        }
    }

    private void RangedAttack()
    {
        // Instantiate the projectile and set its direction
        if (projectilePrefab != null && projectileSpawnPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null && Player != null)
            {
                Vector3 direction = (Player.position - projectileSpawnPoint.position).normalized;
                projectile.transform.rotation = Quaternion.LookRotation(direction);
                projectile.transform.Rotate(90, 0, 0);
                rb.velocity = direction * BulletSpeed;
            }
        }

        // Disable the aiming line when it shooted
        if (aimingLine != null)
        {
            aimingLine.enabled = false;
        }

        //ShootingGapSec = FixShootingGapSec;
    }

    private void StartChasingPlayer()
    {
        isChasingPlayer = true;

        if (enemyAnim != null)
            enemyAnim.SetBool("isWalking", true);

        RotateToFacePlayer();
    }


    // Rotate to face the player
    private void RotateToFacePlayer()
    {
        if (Player != null)
        {
            // Calculate the direction to the player
            Vector3 directionToPlayer = (Player.position - transform.position).normalized;

            // Adjust the direction to include your custom rotation offset (RotationValue in degrees)
            float adjustedYRotation = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg + RotationValue;

            // Create the target rotation with the adjusted Y angle
            Quaternion lookRotation = Quaternion.Euler(0, adjustedYRotation, 0);

            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void SetNewRandomDestination()
    {
        // Random point within the patrol radius
        Vector3 randomPoint = patrolCenter.position + UnityEngine.Random.insideUnitSphere * patrolRadius;

        // Choose a random point and walk to it
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            targetPosition = hit.position;
            if (enemyAnim != null)
                enemyAnim.SetBool("isWalking", true);
            agent.SetDestination(targetPosition);
        }
    }

    private void Patrol()
    {
        // If the agent is close to the current target position, set a new destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTime)
            {
                SetNewRandomDestination();
                idleTimer = 0f;
            } else
            {
                if (enemyAnim != null)
                    enemyAnim.SetBool("isWalking", false);
            }
        }
    }

    /// <summary>
    /// Call whem getting hit
    /// </summary>
    public void OnHit()
    {/*
        // Change the color to red
        enemyMaterial.DOColor(hitColor, "_BaseColor", colorChangeDuration) 
            .OnComplete(() =>
            {
                enemyMaterial.DOColor(originalColor, "_BaseColor", colorChangeDuration);
            });*/
    }

    public void Dead()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }



    private bool PlayerInDetectionRadius(float radius)
    {
        if (Player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, Player.position);
            return distanceToPlayer <= radius;
        }
        return false;
    }

    private void StopChasingPlayer()
    {
        isChasingPlayer = false;
    }


    /// <summary>
    /// For Debug, to see the Random walk around range,  Can Comment All if dont need
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw the patrol radius in the Scene view for debugging
        if (patrolCenter != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(patrolCenter.position, patrolRadius);
        }
    }
}
