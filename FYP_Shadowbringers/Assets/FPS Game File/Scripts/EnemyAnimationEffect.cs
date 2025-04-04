using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEffect : MonoBehaviour
{
    public Collider attackcollider;

    public void EnableCollider()
    {
        attackcollider.enabled = true;
    }

    public void DisableCollider()
    {
        attackcollider.enabled = false;
    }
}
