using System.Collections;
using UnityEngine;

public class ImpactCircle : MonoBehaviour
{
    [SerializeField] private Transform innerCircle; // �����Ρ]�|��j�^
    [SerializeField] private Transform outerCircle; // �~��]�T�w�j�p�^
    [SerializeField] private float initialScale = 0f; // ��l�j�p�]�q���߶}�l�^
    [SerializeField] private float maxScale = 8f; // �̤j�j�p�]���P OuterCircle ���j�p�@�P�^
    [SerializeField] public int beatsToExpand = 4; // ��j�һݪ���l�ƶq�]�i�վ����ס^
    [SerializeField] private float damage = 10f; // �缾�a�y�����ˮ`
    [SerializeField] private Color startColor = new Color(1f, 0f, 0f, 0.3f); // �_�l�C��]�b�z������^
    [SerializeField] private Color endColor = new Color(1f, 0f, 0f, 1f); // �����C��]������^

    private float currentScale;
    private int currentBeatCount = 0; // ��e��l�p��
    private bool isExpanding = false;
    private Material circleMaterial;
    private float outerCircleScale; // �x�s OuterCircle ���j�p
    private float scalePerBeat; // �C�窺�Y��W�q
    private bool hasProcessedBeat = false; // �s�W�ܶq�A�T�O�C�Ӹ`��u�B�z�@��

    private void Awake()
    {
        // �ˬd���n�ե�
        if (innerCircle == null)
        {
            Debug.LogError("ImpactCircle: innerCircle is not set!", this);
            return;
        }
        if (outerCircle == null)
        {
            Debug.LogError("ImpactCircle: outerCircle is not set!", this);
            return;
        }
        // ��l�Ƭ����Ϊ��j�p
        currentScale = initialScale;
        innerCircle.localScale = new Vector3(initialScale, innerCircle.localScale.y, initialScale);

        // ��������Ϊ�����
        Renderer renderer = innerCircle.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("ImpactCircle: innerCircle does not have a Renderer component!", this);
            return;
        }
        circleMaterial = renderer.material;
        if (circleMaterial == null)
        {
            Debug.LogError("ImpactCircle: innerCircle does not have a material!", this);
            return;
        }
        circleMaterial.color = startColor;

        // ��� OuterCircle ���j�p�]���] X �M Z �b�Y��ۦP�^
        outerCircleScale = outerCircle.localScale.x;

        // �p��C�窺�Y��W�q
        scalePerBeat = (maxScale - initialScale) / beatsToExpand;

    }

    public void StartExpanding()
    {
        isExpanding = true;
        currentBeatCount = 0;
    }

    private void Update()
    {
        if (!isExpanding) return;

        if (BeatManager.Instance == null)
        {
            Debug.LogWarning("BeatManager instance is null! Please ensure BeatManager is present in the scene.", this);
            return;
        }

        if (BeatManager.Instance.inBeat)
        {
            if (!hasProcessedBeat)
            {
                BeatManager.Instance._CheckBeat();

                currentBeatCount++;
                Debug.Log($"Beat {currentBeatCount}/{beatsToExpand}, Scale: {currentScale}");
                float t = (float)currentBeatCount / beatsToExpand;
                currentScale = Mathf.Lerp(initialScale, maxScale, t);
                innerCircle.localScale = new Vector3(currentScale, innerCircle.localScale.y, currentScale);

                circleMaterial.color = Color.Lerp(startColor, endColor, t);

                if (currentScale >= outerCircleScale)
                {
                    Debug.Log("ImpactCircle reached max scale, triggering Explode!");
                    Explode();
                    StartCoroutine(DelayedDestroy(0.1f));
                }

                hasProcessedBeat = true;
            }
        }
        else
        {
            hasProcessedBeat = false;
        }
    }

    private IEnumerator FadeOutCircle(Transform circle, float duration)
    {
        Renderer renderer = circle.GetComponent<Renderer>();
        if (renderer == null) yield break;

        Material mat = renderer.material;
        Color startColor = mat.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / duration);
            mat.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        circle.gameObject.SetActive(false);
    }

    private void Explode()
    {
        Boss_Ai boss = FindObjectOfType<Boss_Ai>();
        if (boss != null)
        {
            Debug.Log("Spawning ExplosionVFX at position: " + transform.position);
            boss.GetExplosionVFXFromPool(transform.position);
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
                    Debug.Log($"Player hit by explosion! Position: {hit.transform.position}, Explosion Center: {transform.position}, Explosion Radius: {explosionRadius}");
                }
                else
                {
                    Debug.Log("���a�b�z���d�򤺡A���ʤ� PlayerHealth �ե�I");
                }
            }
        }

        isExpanding = false;
    }

    private IEnumerator DelayedDestroy(float delay)
    {
        yield return new WaitForSeconds(0.1f); // �Y�u����A�]�� ExplosionVFX �w�W�ߺ޲z
        Destroy(gameObject);
    }

    public void SetPosition(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 1f, Vector3.down, out hit, 2f, LayerMask.GetMask("Ground")))
        {
            transform.position = hit.point;
        }
        else
        {
            transform.position = position;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void OnDestroy()
    {
        if (circleMaterial != null)
        {
            Destroy(circleMaterial);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // ø�s�i���d��]����^
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxScale / 2f);

        // ø�s�z���d��]�Ŧ�^
        Gizmos.color = Color.blue;
        float explosionRadius = maxScale / 2f; // �P Explode ��k�����p��@�P
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    public void SetBeatsToExpand(int beats)
    {
        beatsToExpand = beats;
        scalePerBeat = (maxScale - initialScale) / beatsToExpand;
    }
}