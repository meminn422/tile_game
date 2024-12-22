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
    public Transform leftFootTarget; // 左腳目標點
    public Transform rightFootTarget; // 右腳目標點

    // public Transform groundCheck; // 用於檢測地面的空物件
    // public float groundCheckRadius; // 地面檢測的半徑
    // public LayerMask groundLayer; // 哪些層是地面

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

         // 獲取角色的 SpriteRenderer
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
    }
    private void FixedUpdate()
    {
        float move = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
        animator.SetFloat("idle", Mathf.Abs(move));
        spriteRenderer.flipX = move == 0 ? spriteRenderer.flipX : (move > 0 ? false : true);
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
    //腳部ik
    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // 設定左腳目標點
            SetFootIK(AvatarIKGoal.LeftFoot, leftFootTarget);

            // 設定右腳目標點
            SetFootIK(AvatarIKGoal.RightFoot, rightFootTarget);
        }
    }
    private void SetFootIK(AvatarIKGoal foot, Transform target)
    {
        RaycastHit2D hit = Physics2D.Raycast(target.position, Vector2.down, 1f, ground);

        if (hit.collider != null)
        {
            // 將 IK 設置到檢測到的地面位置
            animator.SetIKPosition(foot, hit.point);
            animator.SetIKPositionWeight(foot, 1.0f);
        }
        else
        {
            // 如果沒有檢測到地面，將權重設為 0
            animator.SetIKPositionWeight(foot, 0.0f);
        }
    }
}
