using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitShootingEnemy : MonoBehaviour
{
    public ShootingAi EnemyHealth;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerBullets")
        {
            EnemyHealth.TakeDamage(20);
            Debug.Log("Hit");
        }
    }
}
