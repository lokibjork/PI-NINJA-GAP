using UnityEngine;

public class BossMinerador : EnemyBase
{
    [Header("Configura��es")]
    public float walkSpeed = 2f;
    public float jumpForce = 50f;
    public int groundPoundDamage = 2;
    public float boomerangSpeed = 8f;
    public GameObject boomerangPrefab;
    public Transform throwPoint;

    [Header("Fase 2")]
    public bool isPhase2 = false;
    public float phase2BoomerangSpeed = 12f;
    public float phase2JumpRate = 1.5f;

    private Animator anim;
    public Transform player;
    private bool isGrounded = true;

    void Update()
    {
        if (isGrounded && !isPhase2)
        {
            // Movimento b�sico em dire��o ao player (mas mant�m dist�ncia)  
            float direction = player.position.x > transform.position.x ? 1 : -1;
            rb.linearVelocity = new Vector2(direction * walkSpeed, rb.linearVelocity.y);
        }

        // Ativa Fase 2 quando vida <= 50%  
        if (maxHealth <= 50)
        {
            EnterPhase2();
        }
    }

    void JumpAttack()
    {
        if (isGrounded)
        {
            isGrounded = false;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // Lança a picareta APÓS o pulo (usar Invoke para timing)  
            Invoke("ThrowBoomerang", 0.5f);
        }
    }

    void ThrowBoomerang()
    {
        GameObject boomerang = Instantiate(boomerangPrefab, throwPoint.position, Quaternion.identity);
        PicaretaBoomerang boomerangScript = boomerang.GetComponent<PicaretaBoomerang>();
        boomerangScript.speed = isPhase2 ? phase2BoomerangSpeed : boomerangSpeed;
        boomerangScript.direction = (player.position - throwPoint.position).normalized;
        boomerangScript.isPhase2 = isPhase2; // Passa o estado da fase  
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            // Causa dano se o player estiver perto do impacto  
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    hit.GetComponent<DamageHandler>().TakeDamage(groundPoundDamage, transform.position, 4f);
                    if (isPhase2)
                    {
                        // Adiciona onda de choque na Fase 2  
                        hit.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    void EnterPhase2()
    {
        isPhase2 = true;
        anim.SetBool("Phase2", true);
        CancelInvoke("JumpAttack");
        InvokeRepeating("JumpAttack", 1f, phase2JumpRate); // Pula mais r�pido  
    }
}