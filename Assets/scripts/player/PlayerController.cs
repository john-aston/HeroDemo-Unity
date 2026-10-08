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
    
    
    private void Awake()
    {
        inputControl = new PlayerInputControl();
        PhysicsCheck = GetComponent<PhysicsCheck>();//读取physicscheck文件的代码
        playeranimation = GetComponent<PlayerAnimation>();
        rb = GetComponent<Rigidbody2D>();

        inputControl.GamerPlay.Jump.started += Jump;

        inputControl.GamerPlay.Attack.started += PlayerAttack;

    }

    private void PlayerAttack(InputAction.CallbackContext context)
    {
        playeranimation.PlayAttack();
        isAttack = true;

    }


    //测试



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
