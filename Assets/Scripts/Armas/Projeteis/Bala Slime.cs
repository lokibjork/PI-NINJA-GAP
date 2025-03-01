using UnityEngine;
    public class BalaSlime : Projectile
    {
        void Awake()
        {
            // Configurações para o projétil de slime
            speed = 10f;         // Velocidade moderada
            damage = 1;         // Dano baixo
            lifetime = 4f;       // Tempo suficiente para quicar
            isBouncing = true;   // Ativa o comportamento de quique
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