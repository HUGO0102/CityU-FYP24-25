using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitEmeleeEnemy : MonoBehaviour
{
    public MeleeAi EnemyHealth;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerBullets")
        {
            EnemyHealth.TakeDamage(20);
            Debug.Log("Hit");
        }
    }
}
