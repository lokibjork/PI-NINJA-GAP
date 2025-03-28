using UnityEngine;

public class BalaBazuca : Projectile
{
    void Awake()
    {
        // Configurações para o projétil da bazuca
        speed = 15f;          // Lento
        damage = 4;        // Dano alto
        lifetime = 1f;       // Pode ter um tempo de vida maior para simular a trajetória lenta
        isRocket = true;     // Ativa comportamento de foguete (explosão)
        isBouncing = false;
        isBoomerang = false;
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

                enemy.TakeDamage(damage, knockbackDir);
                Destroy(gameObject); // Destroi o projétil após o impacto
            }
        }
    }
}
