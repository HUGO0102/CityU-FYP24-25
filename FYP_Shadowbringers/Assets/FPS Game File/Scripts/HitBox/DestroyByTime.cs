using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
    public float lifetime = 5f;

    private void OnEnable()
    {
        Invoke("ReturnToPool", lifetime);
    }

    private void ReturnToPool()
    {
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