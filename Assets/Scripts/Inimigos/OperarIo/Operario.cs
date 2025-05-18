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
    public float knockbackForces;

    private Transform playerTransform; // Renomeado para clareza
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
        FindPlayer(); // Encontra o jogador no início
    }

    void OnEnable()
    {
        DamageHandler.OnPlayerDeath += HandlePlayerDeath;
    }

    void OnDisable()
    {
        DamageHandler.OnPlayerDeath -= HandlePlayerDeath;
    }

    void HandlePlayerDeath()
    {
        playerTransform = null;
        Debug.Log("Operario: Jogador morreu, parando de rastrear.");
        enabled = false; // Desativa o script do Operario quando o jogador morre
    }

    void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Operario: Não conseguiu encontrar o GameObject do Jogador com a tag 'Player'.");
            enabled = false; // Desativa se o jogador não for encontrado
        }
    }

    void Update()
    {
        if (playerTransform == null) return; // Não faz nada se o jogador não existir

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
        if (playerTransform == null) return; // Segurança extra

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
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

        if (playerTransform != null) // Verifica antes de usar a posição do jogador
        {
            float direction = Mathf.Sign(playerTransform.position.x - transform.position.x);
            Flip(direction);
            rb.linearVelocity = new Vector2(direction * chargeSpeed, rb.linearVelocity.y);

            yield return new WaitForSeconds(chargeTime);

            rb.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(chargeCooldown);
        }

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
                Vector2 normalDaColisao = collision.GetContact(0).normal;
                Vector2 knockbackDirection = -normalDaColisao.normalized;
                damageHandler.TakeDamage(damage, knockbackDirection, knockbackForces); // Use a variável de dano do Operario
            }
        }
    }
}