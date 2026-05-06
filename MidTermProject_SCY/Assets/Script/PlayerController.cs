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
    Animator anim;                                                                              //플레이어 오브젝트에 붙어 있는 Animator 컴포넌트를 저장할 변수
    private float animSpeed;                                                                    //애니메이션 속도값을 부드럽게 바꾸기 위해 따로 저장하는 변수

    private bool isGiant = false;
    private bool isFast = false;
    private SpriteRenderer sr;                                                                  //플레이어의 SpriteRenderer 컴포넌트를 저장하는 변수

    float score;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();                                                        //현재 플레이어 오브젝트에 붙어 있는 Animator 컴포넌트를 가져와서 anim 변수에 넣는 코드
        sr = GetComponent<SpriteRenderer>();
        score = 0f;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        float currentSpeed = isFast ? moveSpeed * 2f : moveSpeed;                               //현재 플레이어의 실제 이동 속도를 계산하는 코드
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);         //플레이어의 속도를 직접 설정하는 코드

        if (moveInput < 0)                                                                      //플레이어가 보는 방향을 바꾸는 코드입니다.
            sr.flipX = true;                                                                    
        else if (moveInput > 0)                                                                 
        {
            sr.flipX = false;                                                                   
        }

        // 바닥 체크
        float checkRadius = isGiant ? 0.5f : 0.3f;                                              //바닥을 체크할 원의 반지름을 정하는 코드
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);   //플레이어가 현재 땅 위에 있는지 검사하는 코드

        animSpeed = Mathf.Lerp(animSpeed, Mathf.Abs(moveInput), 15f * Time.deltaTime);          //애니메이션용 속도값을 현재 값에서 목표값으로 부드럽게 바꾸는 코드
        anim.SetFloat("Speed", Mathf.Abs(moveInput));                                           //애니메이터의 Speed 파라미터에 현재 이동량을 전달하는 코드
        anim.SetBool("isJumping", !isGrounded);                                                 //애니메이터의 isJumping 파라미터를 설정하는 코드
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
            //HighScore.TrySet(SceneManager.GetActiveScene().buildIndex, (int)score);
            StageResultSaver.SaveStage(SceneManager.GetActiveScene().buildIndex, (int)score);
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
            score += 10f;
            score += collision.GetComponent<ItemObject>().GetPoint();
        }

        if (collision.CompareTag("SpeedItem"))
        {
            Debug.Log("이동 속도 아이템 획득!");
            isFast = true;
            Invoke(nameof(ResetSpeed), 3f);
            Destroy(collision.gameObject);
            score += 20f;
            score += collision.GetComponent<ItemObject>().GetPoint();
        }
    }
    void ResetGiant()
    {
        isGiant = false;
        transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);                                                //거대화 효과가 끝났을 때 플레이어의 크기를 원래대로 되돌리는 코드
    }

    void ResetSpeed()
    {
        Debug.Log("속도 정상화");
        isFast = false;
    }

    private void OnDrawGizmosSelected()                                                                         //유니티 에디터에서 오브젝트를 선택했을 때만 보조선을 그려 주는 함수
    {                                                                               
        if (groundCheck == null) return;                                                                        //groundCheck가 연결되지 않았을 때 함수를 바로 끝내는 코드

        Gizmos.color = Color.red;                                                                               //Scene 뷰에 그릴 보조선의 색을 빨간색으로 지정
        float checkRadius = isGiant ? 0.4f : 0.2f;                                                              //Gizmo로 그릴 바닥 체크 원의 반지름을 정하는 코드
        Gizmos.DrawWireSphere(groundCheck.position, 0.4f);                                                      //groundCheck.position 위치에 반지름 0.4f인 원을 Scene 뷰에 그리는 코드
    }
}
