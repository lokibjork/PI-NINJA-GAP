using System;
using System.Collections;
using Player;
using UnityEngine;

public class Operario : EnemyBase
{
    public float patrolSpeed = 2f;
    public Transform[] patrolPoints; // Array de pontos de patrulha
    public float chargeSpeed = 6f;
    public float detectionRange = 5f;
    public float chargeCooldown = 2f;
    public float chargeTime = 1f;
    public int damage = 1;
    public float patrolPointReachedThreshold = 0.1f; // Distância para considerar o ponto alcançado
    public float idleTimeAtPatrolPoint = 1f; // Tempo de espera em cada ponto

    private Transform player;
    private Transform operarioTransform;
    private bool isCharging = false;
    private bool canCharge = true;
    private int currentPatrolIndex = 0;
    private bool isIdling = false;
    private float idleTimer = 0f;

    new void Start()
    {
        base.Start();
        currentHealth = maxHealth;
        operarioTransform = gameObject.GetComponent<Transform>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Inicializa a posição se houver pontos de patrulha
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            transform.position = patrolPoints[0].position;
        }
    }

    void Update()
    {
        if (!isCharging)
        {
            if (!isIdling)
            {
                Patrol();
            }
            else
            {
                IdleAtPatrolPoint();
            }
            DetectPlayer();
        }
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform targetPatrolPoint = patrolPoints[currentPatrolIndex];
        float distanceToPoint = Vector2.Distance(transform.position, targetPatrolPoint.position);

        Vector2 directionToPoint = (targetPatrolPoint.position - transform.position).normalized;
        Flip(Mathf.Sign(directionToPoint.x));
        rb.linearVelocity = new Vector2(directionToPoint.x * patrolSpeed, rb.linearVelocity.y);

        if (distanceToPoint <= patrolPointReachedThreshold)
        {
            rb.linearVelocity = Vector2.zero;
            isIdling = true;
            idleTimer = 0f;
        }
    }

    void IdleAtPatrolPoint()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleTimeAtPatrolPoint)
        {
            isIdling = false;
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length; // Vai para o próximo ponto
        }
    }

    void DetectPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange && canCharge && !isCharging)
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
        Flip(direction);
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
            if (damageHandler != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                damageHandler.TakeDamage(damage, knockbackDirection, 10f);
            }
        }
    }
}