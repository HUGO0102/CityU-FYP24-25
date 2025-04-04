using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitShootingEnemy : MonoBehaviour
{
    public ShootingAi EnemyHealth;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerBullets")
        {
            EnemyHealth.TakeDamage(20);
            Debug.Log("Hit");
        }
    }
}
