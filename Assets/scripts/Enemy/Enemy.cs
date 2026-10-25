using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    protected Animator anim;
    PhysicsCheck physicsCheck;

    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    public float currentSpeed;
    public Vector3 faceDir;
    public float hurtForce;

    public Transform attacker;

    [Header("计时器")]
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;

    [Header("受伤")]
    public bool isHurt;
    public float hurtTime = 0.7f;      // ★ 略大于受击动画(0.667s)
    private Coroutine hurtRoutine;
    public bool isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();

        currentSpeed = normalSpeed;
        waitTimeCounter = waitTime;
        GetComponent<Character>().OnTakeDamage.AddListener(OnTakeDamage);
    }

    private void Update()
    {
        faceDir = new Vector3(transform.localScale.x, 0, 0);

        if (isHurt) return;            // ★ 受伤期间不跑 AI

        // 撞墙 → 等待
        if ((physicsCheck.touchLeftWall && faceDir.x > 0) || (physicsCheck.touchRightWall && faceDir.x < 0))
            wait = true;

        TimeCounter();

        // ★ 走路动画只看实际速度，不再手动 SetBool(true)
        anim.SetBool("walk", Mathf.Abs(rb.velocity.x) > 0.05f);
    }

    private void FixedUpdate()
    {
        if (isHurt) return;            // ★ 不接管速度，让击退冲量生效

        if (wait)                      // 等待时刹住
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            return;
        }

            Move();
    }

    public virtual void Move()
    {
        rb.velocity = new Vector2(-currentSpeed * faceDir.x, rb.velocity.y);
    }

    public void TimeCounter()
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(-faceDir.x, 1, 1);
            }
        }
        else
        {
            waitTimeCounter = waitTime;
        }
    }

    public void OnTakeDamage(Transform attackTrans)
    {
        if (attackTrans == null) return;
        attacker = attackTrans;

        // 1) 取消撞墙等待，避免两个转身指令打架
        wait = false;
        waitTimeCounter = waitTime;

        // 2) 面向攻击者
        transform.localScale = new Vector3(
            attackTrans.position.x > transform.position.x ? -1f : 1f, 1f, 1f);

        // 3) 动画
        anim.SetBool("walk", false);
        anim.SetTrigger("hurt");

        // 4) 硬直 + 击退（连续挨打时重置协程，避免多个计时器打架）
        if (hurtRoutine != null) StopCoroutine(hurtRoutine);
        hurtRoutine = StartCoroutine(HurtRoutine(attackTrans));
    }

    private IEnumerator HurtRoutine(Transform attackTrans)
    {
        isHurt = true;

        // 先清掉横向速度，再施加"远离攻击者"的冲量
        rb.velocity = new Vector2(0f, rb.velocity.y);

        float dirX = transform.position.x - attackTrans.position.x;   // 正数=攻击者在左
        dirX = Mathf.Approximately(dirX, 0f) ? -transform.localScale.x : Mathf.Sign(dirX);
        rb.AddForce(new Vector2(dirX * hurtForce, 0f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(hurtTime);

        isHurt = false;
        hurtRoutine = null;
    }

    public void OnDied()
    {
        gameObject.layer = 2;    // Ignore Raycast
        anim.SetBool("dead", true);
        isDead = true;
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }
}