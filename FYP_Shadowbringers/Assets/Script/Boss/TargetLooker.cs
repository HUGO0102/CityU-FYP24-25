using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLooker : MonoBehaviour
{
    [SerializeField]
    public Transform targetTrans;
    void LateUpdate()
    {
        if (targetTrans == null)
        {
            transform.localPosition = new Vector3(-0.2f, 0, 0.5f);
        }
        else
        {
            transform.position = targetTrans.transform.position;
        }
    }


}
