using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitEmeleeEnemy : MonoBehaviour
{
    public MeleeAi EnemyHealth;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerBullets")
        {
            EnemyHealth.TakeDamage(10);
            Debug.Log("Hit");
        }

        if (collision.gameObject.tag == "PlayerBrustBullets")
        {
            EnemyHealth.TakeDamage(25);
            Debug.Log("Hit");
        }
    }
}
