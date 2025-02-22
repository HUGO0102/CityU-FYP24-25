using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;

public class Enemy : MonoBehaviour
{
    [Header("Manger")]
    public NavMeshAgent agent;

    [Header("State")]
    public float health = 5;
    private float maxhealth;

    public float startSpeed;
    public float speed;


    [Header("WalkAroundSetting")]
    private Transform patrolCenter; // Center point of the WalkAround area
    private float patrolRadius; // Radius of the WalkAround area

    [Header("Player Detecter")]
    public float detectionRadius = 5f;

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

    private bool isChasingPlayer;

    public Animator enemyAnim;

    public event Action OnDeath;

    // Start is called before the first frame update
    void Start()
    {
        patrolCenter = EnemyManager.Instance.SpawnCenter;

        patrolRadius = EnemyManager.Instance.spawnRadius;
        agent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        enemyRenderer = GetComponent<Renderer>();
        //enemyMaterial = enemyRenderer.material;
        //originalColor = enemyMaterial.color;

        startSpeed = agent.speed;
        speed = startSpeed;

        maxhealth = health;

        //enemyAnim = GetComponent<Animator>();
    }

    void Update()
    {
        if (health <= 0)
        {
            Dead();
        }

        // Check if the player is walked in the detection radius
        if (PlayerInDetectionRadius())
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
                    agent.SetDestination(Player.position);
                }
                else
                {
                    //Enemy is Close to Player
                    //Can Do Enemy Attact Here
                }
            }     
        }
    }

    private void StartChasingPlayer()
    {
        isChasingPlayer = true;

        enemyAnim.SetBool("isWalking", true);


        // Rotate to face the player
        Vector3 directionToPlayer = (Player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }


    private void SetNewRandomDestination()
    {
        // Random point within the patrol radius
        Vector3 randomPoint = patrolCenter.position + UnityEngine.Random.insideUnitSphere * patrolRadius;

        // Choose a random point and walk to it
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            targetPosition = hit.position;
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


    private bool PlayerInDetectionRadius()
    {
        // Check if the player is within the detection radius
        if (Player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, Player.position);
            return distanceToPlayer <= detectionRadius;
        }
        return false;
    }

    private void StopChasingPlayer()
    {
        isChasingPlayer = false;

        enemyAnim.SetBool("isWalking", false);
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
