using System.Collections;
using Player;
using UnityEngine;

public class EnemyOperario : MonoBehaviour
{
    public float patrolSpeed = 2f;
    public float chargeSpeed = 6f;
    public float detectionRange = 5f;
    public float chargeCooldown = 2f;
    public float chargeTime = 1f;
    
    private Transform player;
    private Rigidbody2D rb;
    private Vector2 originalPosition;
    private bool isCharging = false;
    private bool canCharge = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalPosition = transform.position;
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
        // Lógica de patrulha simples (andar de um lado para o outro)
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

    System.Collections.IEnumerator ChargeAttack()
    {
        isCharging = true;
        canCharge = false;

        rb.linearVelocity = Vector2.zero; // Para antes de carregar o ataque
        yield return new WaitForSeconds(0.5f); // Tempo de "preparo" do ataque

        // Calcula a direção para o jogador, mas só no eixo X
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * chargeSpeed, rb.linearVelocity.y);

        yield return new WaitForSeconds(chargeTime);

        rb.linearVelocity = Vector2.zero; // Para após o ataque
        yield return new WaitForSeconds(chargeCooldown);
        
        isCharging = false;
        canCharge = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerData playerData = collision.gameObject.GetComponent<PlayerData>();
            if (playerData != null)
            {
                //playerData.TakeDamage(damage);
            }
        }
    }
}