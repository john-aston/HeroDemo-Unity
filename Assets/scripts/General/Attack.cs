using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage;
    public float attackRange;
    public float attackRate;

    [Header("只对哪些层生效")]
    public LayerMask targetLayers;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) == 0) return;

        if (other.TryGetComponent(out Character target))
            target.TakeDamage(this);
    }
}
