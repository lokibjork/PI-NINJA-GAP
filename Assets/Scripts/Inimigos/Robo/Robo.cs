using Player;
using UnityEngine;

public class Robo : EnemyBase
{
    public float moveSpeed = 3f;
    public float jumpForce = 5;
    public int damage = 1;
    public string groundTag = "Ground";
    
    private Transform target;
    private bool isGrounded;
    private bool playerIsDetected;
    [SerializeField] private Collider2D detectionCollider; // 🔵 Collider que detecta o player (deve ser setado no Inspector)

    private void Start()
    {
        base.Start();
        maxHealth = 10;
        currentHealth = maxHealth;
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (target != null && playerIsDetected)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    // 🔵 Detecta o player dentro do círculo de detecção
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsDetected = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;

        if (obj.CompareTag("Player"))
        {
            playerIsDetected = true; // Garante que o robô reconheça o player no contato

            PlayerData player = obj.GetComponent<PlayerData>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }

        if (obj.CompareTag(groundTag))
        {
            isGrounded = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // 🔴 Filtramos projéteis para que só colidam com o corpo do robô, e não com a detecção
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsDetected = true;
        }
        
        if (collision.CompareTag("Bala"))
        {
            if (collision.IsTouching(detectionCollider)) // Se o projétil atingir o Collider de detecção, ignoramos
            {
                Physics2D.IgnoreCollision(collision, detectionCollider);
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(groundTag))
        {
            isGrounded = false;
        }
    }
}
