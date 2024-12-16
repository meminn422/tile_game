using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb; // Rigidbody2D 組件的引用
    public Animator animator; // Animator 組件的引用
    public SpriteRenderer spriteRenderer; // SpriteRenderer 組件的引用
    public GameObject jc; //empty object
    public LayerMask ground; //map
    public float speed = 5f; // 水平移動速度
    public float jumpSpeed = 15f; // 垂直跳躍速度
    public bool isJump; // 用來檢查玩家是否在跳躍
    public bool isJumpCs=true;
    public int jumpIndex=1;
    public int jumpIndexcs=1;
    public bool isGround;

    private void Awake()
    {
        // 初始化組件
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // 檢查是否按下跳躍按鍵
        if (Input.GetButtonDown("Jump")) 
        {
            isJump = true;            
        }
        isGround = Physics2D.OverlapCircle(jc.transform.position, 0.1f, ground);
        // 獲取水平輸入
        float move = Input.GetAxisRaw("Horizontal");

        // 設置 Animator 的 "Speed" 參數，用於控制動畫
        animator.SetFloat("Speed", Mathf.Abs(move));

        // 根據移動方向翻轉角色的精靈圖
        if (move > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (move < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void FixedUpdate()
    {
        // 控制水平移動
        float move = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
        animator.SetFloat("idle", Mathf.Abs(move));
        spriteRenderer.flipX = move == 0 ? spriteRenderer.flipX : (move > 0 ? false : true);


        if(isGround){
            animator.SetBool("fall", false);
            animator.SetBool("jump", false);
            jumpIndexcs=jumpIndex;
        }
        // 處理跳躍邏輯
        if (isJump && jumpIndexcs > 0)
        { 
            
            animator.SetBool("jump", true);
            animator.SetBool("fall", false);

            jumpIndexcs--;

            isJump = false;
            isGround=false; 
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed); // 應用跳躍的垂直速度
        }
        if (rb.velocity.y < 0)
        {
            animator.SetBool("fall", true);
            animator.SetBool("jump", false);

        }
    }
}
