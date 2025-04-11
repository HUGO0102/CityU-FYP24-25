using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ShieldImpact : MonoBehaviour
{
    public GameObject shieldRipples;

    private VisualEffect shieldRipplesVFX;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with: " + collision.gameObject.tag); // 調試用，檢查標籤
        if (collision.gameObject.tag == "PlayerBullets")
        {
            var ripples = Instantiate(shieldRipples, transform) as GameObject;
            shieldRipplesVFX = ripples.GetComponent<VisualEffect>();

            // 直接從 Collision 對象獲取碰撞點（世界坐標）
            Vector3 closestPoint = collision.contacts[0].point;
            // 將世界坐標轉換為本地坐標（相對於 shieldRipples 對象）
            Vector3 localPoint = ripples.transform.InverseTransformPoint(closestPoint);
            Debug.Log("World Point: " + closestPoint + ", Local Point: " + localPoint); // 調試用
            shieldRipplesVFX.SetVector3("SphereCenter", localPoint);

            Destroy(ripples, 2);
        }
    }
}