using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitBoss : MonoBehaviour
{
    public Boss_Ai EnemyHealth;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerBullets")
        {
            EnemyHealth.TakeDamage(20);
            Debug.Log("Hit Boss with Player Bullet");
        }
    }
}