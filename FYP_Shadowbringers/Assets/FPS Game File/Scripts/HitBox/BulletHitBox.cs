using UnityEngine;

public class BulletHitBox : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitEffect; // 擊中敵人或玩家的效果
    [SerializeField] private ParticleSystem shieldHitEffect; // 擊中護盾的效果

    // 處理玩家子彈的物理碰撞（使用 OnCollisionEnter）
    private void OnCollisionEnter(Collision collision)
    {
        // 僅處理玩家子彈
        if (gameObject.CompareTag("PlayerBullets"))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (hitEffect != null)
                {
                    ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
            else if (collision.gameObject.CompareTag("Shield"))
            {
                if (shieldHitEffect != null)
                {
                    // 使用碰撞點作為粒子效果的位置
                    Vector3 contactPoint = collision.contacts[0].point;
                    ParticleSystem effect = Instantiate(shieldHitEffect, contactPoint, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
        }
    }

    // 處理敵人子彈的觸發器碰撞（使用 OnTriggerEnter）
    private void OnTriggerEnter(Collider other)
    {
        // 僅處理敵人子彈
        if (gameObject.CompareTag("EnemyBullets"))
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Untagged"))
            {
                if (hitEffect != null)
                {
                    ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
            else if (other.gameObject.CompareTag("Shield"))
            {
                if (shieldHitEffect != null)
                {
                    // 因為是觸發器碰撞，無法直接獲取碰撞點，使用子彈位置作為近似
                    Vector3 contactPoint = transform.position;
                    ParticleSystem effect = Instantiate(shieldHitEffect, contactPoint, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
        }
    }

    private void ReturnToPool()
    {
        //Debug.Log("Bullet Returned to Pool: " + gameObject.tag);
        if (BulletPoolManager.Instance != null)
        {
            if (gameObject.CompareTag("PlayerBullets"))
            {
                BulletPoolManager.Instance.ReturnPlayerBullet(gameObject);
            }
            else if (gameObject.CompareTag("EnemyBullets"))
            {
                BulletPoolManager.Instance.ReturnEnemyBullet(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}