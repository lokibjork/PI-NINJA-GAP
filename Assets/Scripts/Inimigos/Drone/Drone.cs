using UnityEngine;
using Player;

public class Drone : EnemyBase
{
    public float moveSpeed = 3f;        // Velocidade de movimentação
    public float hoverHeight = 2f;      // Altura ideal para voar acima do player
    public float circleRadius = 1.5f;   // Raio do círculo ao redor do player
    public float circleSpeed = 2f;      // Velocidade da rotação em torno do player
    public int damage = 5;              // Dano ao encostar no player

    private Transform player;
    private float angle = 0f;           // Ângulo para circular o player

    new void Start()
    {
        base.Start();
        maxHealth = 10; // Bastante vida
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        // Aumenta o ângulo para fazer o drone girar
        angle += circleSpeed * Time.deltaTime;

        // Calcula a posição alvo com base no ângulo e na posição do player
        Vector2 targetPosition = new Vector2(
            player.position.x + Mathf.Cos(angle) * circleRadius,
            player.position.y + hoverHeight + Mathf.Sin(angle) * (circleRadius / 2) // Pequena variação na altura
        );

        // Move o drone para a posição alvo
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerData playerData = collision.GetComponent<PlayerData>();
            if (playerData != null)
            {
                playerData.TakeDamage(5); // 5 de dano e força de 3
            }
        }
    }
}