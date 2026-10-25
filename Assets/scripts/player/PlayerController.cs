using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{

    public PlayerInputControl inputControl;

    private Rigidbody2D rb;
    
    private PhysicsCheck PhysicsCheck;

    public Vector2 inputDirection;
    [Header("基本参数")]
    public float speed;
    public float jumpforce;
    public PlayerAnimation playeranimation;
    public bool isHurt;
    public float hurtForce;
    public bool isDead;
    public bool isAttack;
    
    [Header("连招")]
    public int comboStep;              // 当前连到第几段
    public float comboWindow = 0.8f;   // 超过这么久没按攻击，就从第一段重新开始

    private float lastAttackTime = -999f;
    private Attack[] attackHitboxes;   // 三个攻击盒上的 Attack 组件
    
    
    private void Awake()
    {
        inputControl = new PlayerInputControl();
        PhysicsCheck = GetComponent<PhysicsCheck>();
        playeranimation = GetComponent<PlayerAnimation>();
        rb = GetComponent<Rigidbody2D>();

        // true = 连未激活的子物体一起找（Attack1/2/3 平时是关闭的）
        attackHitboxes = GetComponentsInChildren<Attack>(true);

        inputControl.GamerPlay.Jump.started += Jump;
        inputControl.GamerPlay.Attack.started += PlayerAttack;
    }

    private void PlayerAttack(InputAction.CallbackContext context)
    {
        // 1) 决定这次打第几段
        if (Time.time - lastAttackTime > comboWindow)
            comboStep = 1;                                    // 隔太久 → 重新从第一段起
        else
            comboStep = comboStep >= 3 ? 1 : comboStep + 1;    // 否则接下一段，三段后回到第一段

        lastAttackTime = Time.time;

        // 2) 开始新的一刀，清空命中记录（配合 Attack.cs 的 ResetHitRecord）
        foreach (var hitbox in attackHitboxes)
            hitbox.ResetHitRecord();

        // 3) 播动画
        playeranimation.PlayAttack(comboStep);
        isAttack = true;
    }


    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void Update()
    {
        inputDirection = inputControl.GamerPlay.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if(!isHurt)
        Move();
    }

    //测试
    // private void OnTriggerStay2D(Collider2D other)
    //{
    //  Debug.Log(other.name);
    //}

    public void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

        int faceDir = (int)transform.localScale.x;
        if (inputDirection.x > 0) faceDir = 1;

        if (inputDirection.x < 0) faceDir = -1;
        //人物翻转
        transform.localScale = new Vector3(faceDir , 1 , 1);

    }

    private void Jump(InputAction.CallbackContext context)
    {
        //Debug.Log("jump");测试代码的常用方法

        if(PhysicsCheck.isGround)
            rb.AddForce(transform.up * jumpforce, ForceMode2D.Impulse);

        
    }
    
    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;

        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }



    public void PlayerDead()
    {
        isDead = true;
        inputControl.GamerPlay.Disable();
    }
}
