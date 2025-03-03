using System.Collections;
using Player;
using UnityEngine;

public class Operario : EnemyBase
{
    public float patrolSpeed = 2f;
    public float chargeSpeed = 6f;
    public float detectionRange = 5f;
    public float chargeCooldown = 2f;
    public float chargeTime = 1f;
    
    private Transform player;
    private new Rigidbody2D rb;
    private bool isCharging = false;
    private bool canCharge = true;
    private bool playerDetected = false; // NOVO: Se detectou o player, ele nunca mais patrulha

    public new void Start()
    {
        base.Start();
        maxHealth = 5; // Bastante vida
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (playerDetected)
        {
            FollowPlayer();
        }
        else if (!isCharging)
        {
            Patrol();
            DetectPlayer();
        }
    }

    void Patrol()
    {
        rb.linearVelocity = new Vector2(patrolSpeed, rb.linearVelocity.y);
    }

    void DetectPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= detectionRange && canCharge)
        {
            playerDetected = true; // NOVO: Ele nunca mais volta a patrulhar
            StartCoroutine(ChargeAttack());
        }
    }

    void FollowPlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * patrolSpeed, rb.linearVelocity.y);
    }

    IEnumerator ChargeAttack()
    {
        isCharging = true;
        canCharge = false;

        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * chargeSpeed, rb.linearVelocity.y);

        yield return new WaitForSeconds(chargeTime);

        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(chargeCooldown);
        
        isCharging = false;
        canCharge = true;
    }

    private void OnCollisionEnter2D(Collision2D collision, int damage)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerData playerData = collision.gameObject.GetComponent<PlayerData>();
            if (playerData != null)
            {
                playerData.TakeDamage(damage);
            }
        }
    }
}
