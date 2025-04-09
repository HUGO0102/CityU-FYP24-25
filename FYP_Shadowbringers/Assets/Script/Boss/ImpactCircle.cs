using System.Collections;
using UnityEngine;

public class ImpactCircle : MonoBehaviour
{
    [SerializeField] private Transform innerCircle; // 紅色圓形（會放大）
    [SerializeField] private Transform outerCircle; // 外圈（固定大小）
    [SerializeField] private float initialScale = 0f; // 初始大小（從中心開始）
    [SerializeField] private float maxScale = 8f; // 最大大小（應與 OuterCircle 的大小一致）
    [SerializeField] public int beatsToExpand = 4; // 放大所需的拍子數量（可調整難度）
    [SerializeField] private float damage = 10f; // 對玩家造成的傷害
    [SerializeField] private Color startColor = new Color(1f, 0f, 0f, 0.3f); // 起始顏色（半透明紅色）
    [SerializeField] private Color endColor = new Color(1f, 0f, 0f, 1f); // 結束顏色（實色紅色）

    private float currentScale;
    private int currentBeatCount = 0; // 當前拍子計數
    private bool isExpanding = false;
    private Material circleMaterial;
    private float outerCircleScale; // 儲存 OuterCircle 的大小
    private float scalePerBeat; // 每拍的縮放增量
    private bool hasProcessedBeat = false; // 新增變量，確保每個節拍只處理一次

    private void Awake()
    {
        // 檢查必要組件
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
        // 初始化紅色圓形的大小
        currentScale = initialScale;
        innerCircle.localScale = new Vector3(initialScale, innerCircle.localScale.y, initialScale);

        // 獲取紅色圓形的材質
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

        // 獲取 OuterCircle 的大小（假設 X 和 Z 軸縮放相同）
        outerCircleScale = outerCircle.localScale.x;

        // 計算每拍的縮放增量
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

        // 調整爆炸範圍，使其與可視範圍一致
        float explosionRadius = maxScale / 2f; // 移除 * 1.5f，半徑與 InnerCircle 一致
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
                    Debug.Log("玩家在爆炸範圍內，但缺少 PlayerHealth 組件！");
                }
            }
        }

        isExpanding = false;
    }

    private IEnumerator DelayedDestroy(float delay)
    {
        yield return new WaitForSeconds(0.1f); // 縮短延遲，因為 ExplosionVFX 已獨立管理
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
        // 繪製可視範圍（紅色）
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxScale / 2f);

        // 繪製爆炸範圍（藍色）
        Gizmos.color = Color.blue;
        float explosionRadius = maxScale / 2f; // 與 Explode 方法中的計算一致
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    public void SetBeatsToExpand(int beats)
    {
        beatsToExpand = beats;
        scalePerBeat = (maxScale - initialScale) / beatsToExpand;
    }
}