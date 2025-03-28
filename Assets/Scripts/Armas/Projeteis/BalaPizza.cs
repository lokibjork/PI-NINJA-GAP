using UnityEngine;
using Player;

public class BalaPizza : Projectile
    {
        private bool hasDamagedPlayer = false; // Evita danos múltiplos ao jogador
        void Awake()
        {
            // Configurações para o projétil de pizza (boomerang)
            speed = 12f;         // Velocidade média
            damage = 3;       
            lifetime = 5f;       
            isBoomerang = true;  
            isBouncing = false;
            isRocket = false;
        }

        // Sobrescreve o OnCollisionEnter2D para tratar o dano ao jogador quando o projétil voltar
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Se colidir com um inimigo, não destrói o projétil nem causa dano (comportamento especial)
            if(collision.gameObject.CompareTag("Enemy"))
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

            // Se o projétil estiver retornando e atingir o jogador, aplica dano
            if (collision.gameObject.CompareTag("Player") && returning && !hasDamagedPlayer)
            {
                DamageHandler damageHandler = collision.gameObject.GetComponent<DamageHandler>();
                if(damageHandler != null)
                {
                    Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                    damageHandler.TakeDamage(1, knockbackDir, 10f);
                    hasDamagedPlayer = true;
                }
                // Opcional: você pode decidir se o projétil deve ser destruído ou continuar
                return;
            }

            // Para outras colisões, opcionalmente você pode refletir ou ignorar
            // Se não for boomerang ou bouncing, destrói o projétil (por precaução)
            if (!isBouncing && !isBoomerang && !isRocket)
            {
                Destroy(gameObject);
            }
        }
    }
