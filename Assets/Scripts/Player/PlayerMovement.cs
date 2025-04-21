using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimentação")]
    public float maxSpeed = 128f;
    public float acceleration = 1024f;
    public float deceleration = 1024f;
    public float jumpForce = 192f;
    public float gravity = 512f;
    public float maxFallSpeed = 512f;

    [Header("Verificação de Solo")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;

    [Header("Ajustes Avançados")]
    public float coyoteTime = 0.2f;
    public float bufferedJumpTime = 0.2f;

    private Rigidbody2D rb;
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
    public ParticleSystem _fall;


    [Header("Audio")]
    [SerializeField] public AudioClip _jumpClip;
    [SerializeField] public AudioClip _fallClip;
    [SerializeField] public AudioClip _runClip;

    [Header("Animator")]
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDashing) return;

        moveInput = Input.GetAxisRaw("Horizontal");
        float targetVelocityX = moveInput * maxSpeed;
        float velocityChange = targetVelocityX - rb.linearVelocity.x;
        float accelerationRate = Mathf.Abs(targetVelocityX) > 0.1f ? acceleration : deceleration;
        rb.linearVelocity += new Vector2(Mathf.Clamp(velocityChange, -accelerationRate * Time.deltaTime, accelerationRate * Time.deltaTime), 0);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y - gravity * Time.deltaTime, -maxFallSpeed));

        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        if (Input.GetButtonDown("Jump"))
            bufferedJumpCounter = bufferedJumpTime;
        else
            bufferedJumpCounter -= Time.deltaTime;

        if (bufferedJumpCounter > 0 && coyoteTimeCounter > 0)
        {
            isJumping = true;
            jumpTimeCounter = 0.2f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            bufferedJumpCounter = 0;
            dust.Play();
            SoundManagerSO.PlaySoundFXClip(_jumpClip, transform.position, 1f);
        }

        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
                anim.SetBool("IsJumping", !isGrounded);
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

        /*if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
        */

        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        if (moveInput != 0)
        {
            dust.Play();
            isFacingRight = moveInput > 0;
        }

        anim.SetFloat("xVelocity", Math.Abs(rb.linearVelocity.x));
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            SoundManagerSO.PlaySoundFXClip(_fallClip, transform.position, 1f);
            isGrounded = true;
            _fall.Play();
            anim.SetBool("IsJumping", !isGrounded);
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
