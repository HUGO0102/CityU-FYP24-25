using UnityEngine;

public class BulletHitBox : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitEffect; // 擊中敵人或玩家的效果
    [SerializeField] private ParticleSystem shieldHitEffect; // 擊中護盾的效果

    private void OnCollisionEnter(Collision collision)
    {
        // 玩家子彈擊中敵人或護盾
        if (gameObject.tag == "PlayerBullets")
        {
            if (collision.gameObject.tag == "Enemy")
            {
                if (hitEffect != null)
                {
                    ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
            else if (collision.gameObject.tag == "Shield")
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
        // 敵人子彈擊中玩家、未標記對象或護盾
        else if (gameObject.tag == "EnemyBullets")
        {
            if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Untagged")
            {
                if (hitEffect != null)
                {
                    ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
            else if (collision.gameObject.tag == "Shield")
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

    private void ReturnToPool()
    {
        Debug.Log("Bullet Returned to Pool: " + gameObject.tag);
        if (BulletPoolManager.Instance != null)
        {
            if (gameObject.tag == "PlayerBullets")
            {
                BulletPoolManager.Instance.ReturnPlayerBullet(gameObject);
            }
            else if (gameObject.tag == "EnemyBullets")
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