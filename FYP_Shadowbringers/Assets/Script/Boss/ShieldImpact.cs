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
        //Debug.Log("Collision with: " + collision.gameObject.tag);
        if (collision.gameObject.tag == "PlayerBullets" || collision.gameObject.tag == "PlayerBrustBullets")
        {
            var ripples = Instantiate(shieldRipples, transform) as GameObject;
            shieldRipplesVFX = ripples.GetComponent<VisualEffect>();

            Vector3 closestPoint = collision.contacts[0].point;
            Vector3 localPoint = ripples.transform.InverseTransformPoint(closestPoint);
            //Debug.Log("World Point: " + closestPoint + ", Local Point: " + localPoint);
            shieldRipplesVFX.SetVector3("SphereCenter", localPoint);

            Destroy(ripples, 2);
        }
    }
}