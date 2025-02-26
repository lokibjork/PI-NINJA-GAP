using UnityEngine;
using Player;

public class BalaPizza : Projectile
    {
        private bool hasDamagedPlayer = false; // Evita danos múltiplos ao jogador
        void Awake()
        {
            // Configurações para o projétil de pizza (boomerang)
            speed = 12f;         // Velocidade média
            damage = 3;        // Dano moderado
            lifetime = 5f;       // Maior tempo de vida para o efeito boomerang
            isBoomerang = true;  // Ativa o comportamento de boomerang (já tratado no Update do base Projectile)
            isBouncing = false;
            isRocket = false;
        }

        // Sobrescreve o OnCollisionEnter2D para tratar o dano ao jogador quando o projétil voltar
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Se colidir com um inimigo, não destrói o projétil nem causa dano (comportamento especial)
            if(collision.gameObject.CompareTag("Enemy"))
            {
                // Você pode adicionar aqui lógica de dano ao inimigo se quiser
                return;
            }

            // Se o projétil estiver retornando e atingir o jogador, aplica dano
            if (collision.gameObject.CompareTag("Player") && returning && !hasDamagedPlayer)
            {
                PlayerData playerData = collision.gameObject.GetComponent<PlayerData>();
                if(playerData != null)
                {
                    playerData.TakeDamage((int)damage);
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
