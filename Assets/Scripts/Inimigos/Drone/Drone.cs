using UnityEngine;
using Player;

public class Drone : EnemyBase
{
    public float moveSpeed = 3f;
    public float hoverHeight = 2f;
    public float circleRadius = 1.5f;
    public float circleSpeed = 2f;
    public int damage = 1;

    private Transform player;
    private float angle = 0f;
    private bool playerDetected = false; // 👈 Novo: começa falso

    new void Start()
    {
        base.Start();
        maxHealth = 4;
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!playerDetected || player == null) return;

        angle += circleSpeed * Time.deltaTime;

        Vector2 targetPosition = new Vector2(
            player.position.x + Mathf.Cos(angle) * circleRadius,
            player.position.y + hoverHeight + Mathf.Sin(angle) * (circleRadius / 2)
        );

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!playerDetected && collision.gameObject.tag == "Player")
        {
            playerDetected = true; // 👈 Começa a seguir o player
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DamageHandler damageHandler = collision.gameObject.GetComponent<DamageHandler>();
            if (damageHandler != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                damageHandler.TakeDamage(1, knockbackDirection, 10f);
            }
        }
    }
}
