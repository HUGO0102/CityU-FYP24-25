using UnityEngine;

public class BulletHitBox : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.tag == "PlayerBullets" && other.gameObject.tag == "Enemy")
        {
            if (hitEffect != null)
            {
                ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                Destroy(effect.gameObject, effect.main.duration);
            }
            ReturnToPool();
        }
        else if (gameObject.tag == "EnemyBullets" && (other.gameObject.tag == "Player" || other.gameObject.tag == "Untagged"))
        {
            if (hitEffect != null)
            {
                ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                Destroy(effect.gameObject, effect.main.duration);
            }
            ReturnToPool();
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