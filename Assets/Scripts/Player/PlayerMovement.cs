using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimentação")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float jumpHoldForce = 2f; // Força extra ao segurar pulo
    public float jumpTime = 0.2f;

    [Header("Verificação de Solo")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;

    private Rigidbody2D rb;
    private Weapon _weapon;
    public bool isGrounded;
    private float jumpTimeCounter;
    private bool isJumping;
    private float moveInput;
    public bool isFacingRight;
    public ParticleSystem dust;
    [SerializeField] public AudioClip _jumpClip;
    [SerializeField] public AudioClip _fallClip;
    [SerializeField] public AudioClip _runClip;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Input de movimento (sem suavização de flip)
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y); // Alterado para velocity em vez de linearVelocity
        
        // Pulo dinâmico
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            dust.Play();
            SoundManagerSO.PlaySoundFXClip(_jumpClip, transform.position, 1f);// Alterado para velocity em vez de linearVelocity
        }
        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce + jumpHoldForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
        
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }
        
        // Flip instantâneo
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        if (moveInput > 0)
        {
            dust.Play();
            isFacingRight = true;
        }
        else if (moveInput < 0)
        {
            dust.Play();
            isFacingRight = false;
        }
    }
    

    // Coloque os métodos de trigger fora do Update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            SoundManagerSO.PlaySoundFXClip(_fallClip, transform.position, 1f);
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
