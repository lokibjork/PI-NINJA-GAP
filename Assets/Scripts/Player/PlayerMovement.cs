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

    public Rigidbody2D rb;
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
    // [SerializeField] public AudioClip _runClip; // Este campo pode ser um array se você tiver múltiplos sons de passo

    // NOVOS CAMPOS PARA O SOM DE ANDAR
    [Header("Som de Andar")]
    [SerializeField]
    [Tooltip("Clipe(s) de áudio para o som de andar/correr.")]
    public AudioClip[] _walkClips; // Pode ser um array para variar os sons
    [Tooltip("Volume do som de andar.")]
    public float walkVolume = 0.5f;
    [Tooltip("Distância que o jogador precisa percorrer no chão para tocar o próximo som de andar.")]
    public float distancePerStepSound = 1f; // Ajuste para a frequência do som
    private Vector2 lastGroundPosition;
    private float distanceTraveledSinceLastSound;
    private bool isPlayingWalkSound = false; // Flag para controlar o som contínuo (se for o caso)

    [Header("Animator")]
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lastGroundPosition = transform.position; // Inicializa a posição para o cálculo de distância
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
        {
            coyoteTimeCounter = coyoteTime;
            // Se estiver no chão e se movendo horizontalmente, calcule a distância
            if (Mathf.Abs(moveInput) > 0.1f)
            {
                distanceTraveledSinceLastSound += Vector2.Distance(transform.position, lastGroundPosition);
                lastGroundPosition = transform.position;

                if (distanceTraveledSinceLastSound >= distancePerStepSound)
                {
                    // Toca um som de andar
                    if (_walkClips != null && _walkClips.Length > 0)
                    {
                        SoundManagerSO.PlaySoundFXClips(_walkClips, transform.position, walkVolume);
                    }
                    distanceTraveledSinceLastSound = 0f; // Reseta a distância
                }
            }
            else
            {
                // Se não estiver se movendo, reseta a distância e a posição
                distanceTraveledSinceLastSound = 0f;
                lastGroundPosition = transform.position;
            }
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            // Reseta a distância quando não estiver no chão
            distanceTraveledSinceLastSound = 0f;
            lastGroundPosition = transform.position;
        }

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

        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        if (moveInput != 0 && isGrounded) // Só cria poeira se estiver no chão e se movendo
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
            lastGroundPosition = transform.position; // Reseta a posição no chão ao tocar o solo
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = false;
            // Quando sai do chão, para de contar a distância e reseta a posição
            distanceTraveledSinceLastSound = 0f;
        }
    }
}