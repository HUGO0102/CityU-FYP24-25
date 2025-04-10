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
        Debug.Log("Trigger with: " + other.gameObject.tag); // �ոեΡA�ˬd����
        if (other.gameObject.tag == "PlayerBullets")
        {
            var ripples = Instantiate(shieldRipples, transform) as GameObject;
            shieldRipplesVFX = ripples.GetComponent<VisualEffect>();
            // �]�� OnTriggerEnter �S�� contacts �I�A�ݭn��ʭp��I���I
            Vector3 closestPoint = GetComponent<Collider>().ClosestPoint(other.transform.position);
            shieldRipplesVFX.SetVector3("SphereCenter", closestPoint);
            Destroy(ripples, 2);
        }
    }
}
