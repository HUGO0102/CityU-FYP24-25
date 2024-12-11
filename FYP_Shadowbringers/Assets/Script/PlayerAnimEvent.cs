using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvent : MonoBehaviour
{
    public bool Attacking;

    public void Attack()
    {
        Attacking = true;
    }

    public void AttackEnd()
    {
        Attacking = false;
    }
}
