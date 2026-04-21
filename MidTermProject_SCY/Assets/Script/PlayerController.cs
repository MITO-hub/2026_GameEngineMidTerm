using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float jumpForce = 3f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isJumping; 
    private float moveInput;                
    Animator anim;                                                                              //
    private float animSpeed;                                                                    //

    private bool isGiant = false;
    private bool isFast = false;
    private SpriteRenderer sr;                                                                  //

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();                                                        //
        sr = GetComponent<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*
        anim = GetComponent<Animator>();                                                        //애니메이터 가져오기
        */
    }

    // Update is called once per frame
    void Update()
    {
        float currentSpeed = isFast ? moveSpeed * 2f : moveSpeed;                               //
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);         //

        /*
        float scale = isGiant ? 2f : 1f;                                                        //
        if (moveInput < 0)                                                                      //
            transform.localScale = new Vector3(-scale, scale, 1);                               //
        else if (moveInput > 0)                                                                 //
            transform.localScale = new Vector3(scale, scale, 1);                                //
        */

        if (moveInput < 0)
            sr.flipX = true;
        else if (moveInput > 0)
        {
            sr.flipX = false;
        }

        // 바닥 체크
        float checkRadius = isGiant ? 0.5f : 0.3f;                                              //
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);   //

        animSpeed = Mathf.Lerp(animSpeed, Mathf.Abs(moveInput), 15f * Time.deltaTime);
        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("isJumping", !isGrounded);

        /*
        if (isGiant)
        {
            if (moveInput < 0)
                transform.localScale = new Vector3(-0.1f, 0.1f, 0.1f);
            else if (moveInput > 0)
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
        else
        {
            if (moveInput < 0)
                transform.localScale = new Vector3(-0.07f, 0.07f, 0.07f);
            else if (moveInput > 0)
                transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
        }
        
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        

        if (isGrounded)
        {
            float h = Input.GetAxisRaw("Horizontal");       //좌우 입력값 (-1, 0, 1)
            anim.SetFloat("Speed", Mathf.Abs(h));       //입력값(절대값)을 애니메이터의 Speed 파라미터에 전달

            isJumping = false;
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            anim.SetTrigger("Player_Jump");
            isJumping = true;
        }
        */
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;
        Debug.Log("moveInput: " + moveInput);
    }

    public void OnJump(InputValue value)
    {
        Debug.Log("Jump 입력 들어옴 / isPressed: " + value.isPressed + " / isGrounded: " + isGrounded);

        if (value.isPressed && isGrounded)
        {
            Debug.Log("점프 실행");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            if (!isGiant)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                Debug.Log("거대화 상태라 함정을 무시합니다!");
            }
        }
        
        if (collision.CompareTag("Finish"))
        {
            collision.GetComponent<LevelObject>().MoveToNextLevel();
        }

        if (collision.CompareTag("Enemy"))
        {
            if (isGiant)
                Destroy(collision.gameObject);
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (collision.CompareTag("Item"))
        {
            Debug.Log("거대화 아이템 획득!");
            isGiant = true;
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Invoke(nameof(ResetGiant), 7f);
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("SpeedItem"))
        {
            Debug.Log("이동 속도 아이템 획득!");
            isFast = true;
            Invoke(nameof(ResetSpeed), 3f);
            Destroy(collision.gameObject);
        }
    }
    void ResetGiant()
    {
        isGiant = false;
        transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);                                                //
    }

    void ResetSpeed()
    {
        Debug.Log("속도 정상화");
        isFast = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        float checkRadius = isGiant ? 0.4f : 0.2f;
        Gizmos.DrawWireSphere(groundCheck.position, 0.4f);
    }
}
