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
        Debug.Log("Collision with: " + collision.gameObject.tag); // �ոեΡA�ˬd����
        if (collision.gameObject.tag == "PlayerBullets")
        {
            var ripples = Instantiate(shieldRipples, transform) as GameObject;
            shieldRipplesVFX = ripples.GetComponent<VisualEffect>();

            // �����q Collision ��H����I���I�]�@�ɧ��С^
            Vector3 closestPoint = collision.contacts[0].point;
            // �N�@�ɧ����ഫ�����a���С]�۹�� shieldRipples ��H�^
            Vector3 localPoint = ripples.transform.InverseTransformPoint(closestPoint);
            Debug.Log("World Point: " + closestPoint + ", Local Point: " + localPoint); // �ոե�
            shieldRipplesVFX.SetVector3("SphereCenter", localPoint);

            Destroy(ripples, 2);
        }
    }
}