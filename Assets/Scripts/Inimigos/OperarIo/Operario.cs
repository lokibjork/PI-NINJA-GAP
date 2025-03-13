using System;
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
    public int damage = 1;

    private Transform player;
    private bool isCharging = false;
    private bool canCharge = true;

    new void Start()
    {
        base.Start();
        maxHealth = 10; // Bastante vida
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!isCharging)
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
            StartCoroutine(ChargeAttack());
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DamageHandler damageHandler = collision.gameObject.GetComponent<DamageHandler>();
            if(damageHandler != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                damageHandler.TakeDamage(1, knockbackDirection, 10f); // Ajuste o dano e força conforme preferir
            }
        }
    }
}
