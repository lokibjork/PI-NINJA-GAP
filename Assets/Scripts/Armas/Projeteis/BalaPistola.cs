using UnityEngine;
using Player;
public class BalaPistola : Projectile
{
    void Awake()
    {
        // Configurações para o projétil básico
        speed = 35f;         // Velocidade média
        damage = 1;         // Dano pequeno
        lifetime = 2f;       // Alcance médio (tempo de vida)
        isBouncing = false;
        isBoomerang = false;
        isRocket = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                // Calcula a direção do knockback baseado na posição do tiro e do inimigo
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;

                enemy.TakeDamage(damage, -knockbackDir);
                Destroy(gameObject); // Destroi o projétil após o impacto
            }
        }

        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("BalaBoss"))
        {
                Destroy(gameObject);
            }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Detect"))
            return;
    }

}