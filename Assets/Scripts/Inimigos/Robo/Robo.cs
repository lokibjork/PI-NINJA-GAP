using UnityEngine;

public class Robo : EnemyBase
{
    public float runSpeed = 3f;
    public float jumpForce = 8f;
    public float detectionRange = 5f;
    public float attackCooldown = 2f;

    private Transform player;
    private bool isAttacking = false;
    private float cooldownTimer = 0f;

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isAttacking)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                isAttacking = false;
            }
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            AttackPlayer();
        }
        else
        {
            RunTowardsPlayer();
        }
    }

    void RunTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * runSpeed, rb.linearVelocity.y);
    }

    void AttackPlayer()
    {
        isAttacking = true;
        cooldownTimer = attackCooldown;

        Vector2 jumpDirection = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(jumpDirection.x * jumpForce, jumpForce);
    }
}