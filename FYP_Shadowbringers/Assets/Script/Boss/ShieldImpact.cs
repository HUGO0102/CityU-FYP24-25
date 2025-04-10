using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ShieldImpact : MonoBehaviour
{
    public GameObject shieldRipples;

    private VisualEffect shieldRipplesVFX;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger with: " + other.gameObject.tag); // 調試用，檢查標籤
        if (other.gameObject.tag == "PlayerBullets")
        {
            var ripples = Instantiate(shieldRipples, transform) as GameObject;
            shieldRipplesVFX = ripples.GetComponent<VisualEffect>();
            // 因為 OnTriggerEnter 沒有 contacts 點，需要手動計算碰撞點
            Vector3 closestPoint = GetComponent<Collider>().ClosestPoint(other.transform.position);
            shieldRipplesVFX.SetVector3("SphereCenter", closestPoint);
            Destroy(ripples, 2);
        }
    }
}
