using UnityEngine;
public class BalaPistola : Projectile
{
    void Awake()
    {
        // Configurações para o projétil básico
        speed = 15f;         // Velocidade média
        damage = 1;         // Dano pequeno
        lifetime = 2f;       // Alcance médio (tempo de vida)
        isBouncing = false;
        isBoomerang = false;
        isRocket = false;
    }
     
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
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