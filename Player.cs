using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5f; // 水平移動速度
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // 控制動畫參數
        float move = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(move));

        // 翻轉角色方向
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
        // 控制角色移動
        float move = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
    }
}
