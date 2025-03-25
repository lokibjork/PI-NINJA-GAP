using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimentação")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float jumpHoldForce = 2f;
    public float jumpTime = 0.2f;
    public float dashSpeed = 15f;
    public float airControl = 0.8f;

    [Header("Verificação de Solo")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;

    [Header("Ajustes Avançados")]
    public float coyoteTime = 0.2f;
    public float bufferedJumpTime = 0.2f;

    private Rigidbody2D rb;
    private Weapon _weapon;
    public bool isGrounded;
    private float jumpTimeCounter;
    private float coyoteTimeCounter;
    private float bufferedJumpCounter;
    public bool isJumping;
    private float moveInput;
    public bool isFacingRight;
    public bool canDash = true;
    public bool isDashing;

    public ParticleSystem dust;
    public ParticleSystem _jump;
    public ParticleSystem _fall;

    [SerializeField] public AudioClip _jumpClip;
    [SerializeField] public AudioClip _fallClip;
    [SerializeField] public AudioClip _runClip;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isDashing) return;

        // Input de movimento com controle no ar
        moveInput = Input.GetAxisRaw("Horizontal");
        float currentSpeed = isGrounded ? moveSpeed : moveSpeed * airControl;
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);

        // Coyote Time
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // Buffered Jump
        if (Input.GetButtonDown("Jump"))
            bufferedJumpCounter = bufferedJumpTime;
        else
            bufferedJumpCounter -= Time.deltaTime;

        // Pulo dinâmico
        if (bufferedJumpCounter > 0 && coyoteTimeCounter > 0)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            bufferedJumpCounter = 0;
            dust.Play();
            SoundManagerSO.PlaySoundFXClip(_jumpClip, transform.position, 1f);
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
            _jump.Play();
            isJumping = false;
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        // Flip instantâneo
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        if (moveInput != 0)
        {
            dust.Play();
            isFacingRight = moveInput > 0;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0f);
        yield return new WaitForSeconds(0.3f);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(1f); // Cooldown do dash
        canDash = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            SoundManagerSO.PlaySoundFXClip(_fallClip, transform.position, 1f);
            isGrounded = true;
            _fall.Play();
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
