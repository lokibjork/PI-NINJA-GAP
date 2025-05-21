using Player;
using System;
using UnityEngine;

public class Mercenary : EnemyBase
{
    public float moveSpeed = 2f;
    public float attackCooldown = 1.5f;
    public float chaseRange = 7f;
    public float attackRange = 3f;
    public int damage = 2;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private Transform player;
    private float cooldownTimer = 0f;

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && cooldownTimer <= 0)
        {
            rb.linearVelocity = Vector2.zero; // Para de andar para atirar
            AttackPlayer();
        }
        else if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }

        cooldownTimer -= Time.deltaTime;
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
    }

    void AttackPlayer()
    {
        cooldownTimer = attackCooldown;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = (player.position - firePoint.position).normalized;
        projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * 5f; // Velocidade do projétil
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DamageHandler damageHandler = collision.gameObject.GetComponent<DamageHandler>();
            if(damageHandler != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                damageHandler.TakeDamage(damage, knockbackDir, 2f);
            }
        }
    }
}