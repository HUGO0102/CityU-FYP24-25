using UnityEngine;

public class MiniImpactCircle : ImpactCircle
{
    protected override void Explode()
    {
        Boss_Ai boss = FindObjectOfType<Boss_Ai>();
        if (boss != null)
        {
            Debug.Log("Spawning MiniExplosionVFX at position: " + transform.position);
            boss.GetMiniExplosionVFXFromPool(transform.position);
        }
        else
        {
            Debug.LogWarning("Boss_Ai not found in the scene!");
        }

        if (innerCircle != null)
        {
            StartCoroutine(FadeOutCircle(innerCircle, 0.5f));
        }
        if (outerCircle != null)
        {
            StartCoroutine(FadeOutCircle(outerCircle, 0.5f));
        }

        // �վ��z���d��A�Ϩ�P�i���d��@�P
        float explosionRadius = maxScale / 2f; // ���� * 1.5f�A�b�|�P InnerCircle �@�P
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log($"Player hit by mini explosion! Position: {hit.transform.position}, Explosion Center: {transform.position}, Explosion Radius: {explosionRadius}");
                }
                else
                {
                    Debug.Log("���a�b�z���d�򤺡A���ʤ� PlayerHealth �ե�I");
                }
            }
        }

        isExpanding = false;
    }
}