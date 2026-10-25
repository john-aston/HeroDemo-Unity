using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;

    private Rigidbody2D rb;

    private PhysicsCheck physicscheck;

    private PlayerController playercontroller;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicscheck = GetComponent<PhysicsCheck>();
        playercontroller = GetComponent<PlayerController>();

    }

    private void Update()
    {
        SetAnimation();
    }

    public void SetAnimation()
    {
        anim.SetFloat("velocityX",Mathf.Abs(rb.velocity.x));
        anim.SetFloat("velocityY",rb.velocity.y);
        anim.SetBool("isGround", physicscheck.isGround);
        anim.SetBool("isDead", playercontroller.isDead);
        anim.SetBool("isAttack", playercontroller.isAttack);
    }

    public void PlayHurt()
    {
        anim.SetTrigger("hurt");
    }

    public void PlayAttack(int step)
    {
        anim.SetInteger("comboStep", step);   // 先写段数
        anim.SetTrigger("attack");            // 再触发过渡
    }
}
