using Player;
using UnityEngine;

public class Robo : EnemyBase
{
    public float moveSpeed = 3f;
    public float jumpForce = 7f;
    public int damage = 5; // Pouco ataque
    public Transform groundCheck;
    public string groundTag = "Ground"; // Tag usada para identificar o chão
    private Transform target;
    private bool isGrounded;

    protected override void Start()
    {
        base.Start();
        maxHealth = 10; // Bastante vida
        currentHealth = maxHealth;
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(groundTag))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(groundTag))
        {
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerData player = collision.gameObject.GetComponent<PlayerData>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
        else if (collision.gameObject.CompareTag(groundTag) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
}