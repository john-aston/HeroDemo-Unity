using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage;

    [Header("只对哪些层生效")]
    public LayerMask targetLayers;

    [Header("同一个目标隔多久才能再次受伤（秒）")]
    public float hitInterval = 0.5f;

    // 每个目标上一次被打中的时间
    private readonly Dictionary<Collider2D, float> lastHitTime = new Dictionary<Collider2D, float>();

    /// <summary>每次按下攻击键时由 PlayerController 调用：开始新的一刀</summary>
    public void ResetHitRecord()
    {
        lastHitTime.Clear();
    }

    // 判定框重新被打开时（玩家挥砍）也重置一次
    private void OnEnable()
    {
        lastHitTime.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryHit(other);
    }

    private void TryHit(Collider2D other)
    {
        // 1) 图层过滤
        if (((1 << other.gameObject.layer) & targetLayers) == 0) return;

        // 2) 还在冷却里就不重复伤害（这样"常开"的判定框也能反复造成伤害）
        if (lastHitTime.TryGetValue(other, out float t) && Time.time - t < hitInterval) return;

        // 3) 对方有 Character 才扣血
        if (other.TryGetComponent(out Character target))
        {
            lastHitTime[other] = Time.time;
            target.TakeDamage(this);
        }
    }
}