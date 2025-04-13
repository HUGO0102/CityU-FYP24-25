using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitBoss : MonoBehaviour
{
    public Boss_Ai EnemyHealth;

    private void Start()
    {
        if (EnemyHealth == null)
        {
            EnemyHealth = GetComponentInParent<Boss_Ai>();
            if (EnemyHealth == null)
            {
                Debug.LogError("Boss_Ai script not found in parent!");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerBullets")
        {
            if (EnemyHealth != null)
            {      
                    EnemyHealth.TakeDamage(20);
                    Debug.Log("Hit Boss with Player Bullet");
            }
            else
            {
                Debug.LogWarning("EnemyHealth is null, cannot apply damage!");
            }
        }
    }
}