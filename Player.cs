using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb; // Rigidbody2D 組件的引用
    public float speed;//player's
    public bool isJump;
    public float jumpSpeed; // 跳躍的力度
    public Animator animator; // Animator 組件的引用
    public SpriteRenderer spriteRenderer; // SpriteRenderer 組件的引用
    private bool isGround; // 是否在地面上
    public GameObject jc; //empty object
    public LayerMask ground; //map

    //左右腳動畫控制
     public Transform characterRoot; // 角色的根骨骼（通常是角色的主Transform）
    public Transform leftFootTarget; // 左腳的IK目標
    public Transform rightFootTarget; // 右腳的IK目標
    public float stepDistance = 0.5f; // 每步的移動距離
    //黏液
    public GameObject slimePrefab; // 黏液的預製體
    public float slimeDuration = 5f; // 黏液持續時間
    public Transform slimeSpawnPoint; // 黏液生成位置
    //隱身
    // public SpriteRenderer spriteRenderer; // 角色的 SpriteRenderer
    public float invisibilityDuration = 10f; // 隱身持續時間
    private bool isInvisible = false; // 是否處於隱身狀態
     private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
     private void Update()
    {
        if (Input.GetButtonDown("Jump")) {
            isJump = true;
        }
        isGround = Physics2D.OverlapCircle(jc.transform.position, 0.1f, ground);
 
        // 技能觸發按鍵
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CastSlime(); // 執行吐黏液技能
        }
        // 當按下隱身技能按鍵
        if (Input.GetKeyDown(KeyCode.E) && !isInvisible)
        {
            StartCoroutine(ActivateInvisibility());
        }
         // 根據角色的位置動態更新 IK 目標
        leftFootTarget.position = characterRoot.position + new Vector3(-stepDistance, 0, 0);
        rightFootTarget.position = characterRoot.position + new Vector3(stepDistance, 0, 0);

        // 確保目標的高度符合地面（可以加入Raycast進行精確調整）
        leftFootTarget.position = new Vector3(leftFootTarget.position.x, 0.5f, leftFootTarget.position.z);
        rightFootTarget.position = new Vector3(rightFootTarget.position.x, 0.5f, rightFootTarget.position.z);
    }
    private void FixedUpdate()
    {
        float move = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
        animator.SetFloat("idle", Mathf.Abs(move));
        // spriteRenderer.flipX = move == 0 ? spriteRenderer.flipX : (move > 0 ? false : true);
        if (move != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = move > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x); // 根據移動方向改變 X 軸方向
            transform.localScale = scale;
        }

         if(isGround){
            // animator.SetBool("fall", false);
            animator.SetBool("jump", false);
            // jumpIndexcs=jumpIndex;
        }
        if (isJump)
        {  
            animator.SetBool("jump", true);
            // animator.SetBool("fall", false);
            // jumpIndexcs--;

            isJump = false;
            isGround=false; 
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed); // 應用跳躍的垂直速度
        }
        if (rb.velocity.y < 0)
        {
            animator.SetBool("jump", false);
        }
    }
    private void CastSlime()
    {
        // 在生成點生成黏液
        GameObject slime = Instantiate(slimePrefab, slimeSpawnPoint.position, Quaternion.identity);

        // 開始計時，10 秒後銷毀黏液
        Destroy(slime, slimeDuration);
    }
    private IEnumerator ActivateInvisibility()
    {
        // 開啟隱身狀態
        isInvisible = true;
        SetTransparency(0f); // 設置透明度，0 表示完全隱身

        // 等待隱身持續時間
        yield return new WaitForSeconds(invisibilityDuration);

        // 恢復正常狀態
        SetTransparency(1f); // 設置回完全顯示
        isInvisible = false;
    }

   public void SetTransparency(float a)
    {
        // 獲取當前物件及所有子物件中的 SpriteRenderer
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // 遍歷所有 SpriteRenderer，設置透明度為 0
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = a; // 設置透明度為 0
                spriteRenderer.color = color;
            }
        }
    }
    
}
