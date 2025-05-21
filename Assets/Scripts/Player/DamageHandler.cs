using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Player
{
    public class DamageHandler : MonoBehaviour
    {
        public PlayerData playerData;
        private bool isInvulnerable = false;
        public float invulnerabilityTime = 0.2f;
        public Rigidbody2D rb;
        public SpriteRenderer spriteRenderer;
        public AudioClip _playerHit;
        public ScreenShaker screenShaker;
        public float shakeIntensity = 0.1f;
        public float shakeDuration = 0.1f;

        // Evento estático para notificar outros scripts sobre a morte do jogador
        public static event System.Action OnPlayerDeath;

        private bool hasDiedThisLife = false; // NOVA FLAG para controlar se o player já morreu nesta "vida"

        void Awake() // Use Awake para garantir que a inicialização ocorra cedo
        {
            // Resetar o estado ao carregar a cena
            hasDiedThisLife = false;
            isInvulnerable = false;

            // Certifique-se que o script esteja ativo
            this.enabled = true;

            // ... suas outras inicializações de referências aqui ...
            if (playerData == null)
            {
                playerData = GetComponent<PlayerData>();
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }

            // Garanta que o SpriteRenderer e Collider estejam ativos para a nova vida
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
                spriteRenderer.color = Color.white; // Reseta a cor para o padrão
            }
            Collider2D playerCollider = GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                playerCollider.enabled = true;
            }
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic; // Garante que o Rigidbody esteja ativo se precisar
                rb.linearVelocity = Vector2.zero; // Zera a velocidade
            }
        }

        public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
        {
            // Use hasDiedThisLife em vez de isDead para controlar a morte nesta vida
            if (isInvulnerable || hasDiedThisLife) return;

            playerData.currentHealth -= damage;
            playerData.healthBar.SetHealth(playerData.currentHealth);

            // Efeito sonoro de dano
            if (_playerHit != null)
            {
                //SoundManagerSO.PlaySoundFXClip(_playerHit, transform.position, 1f); // Certifique-se que SoundManagerSO é acessível
            }

            // Knockback
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
            }

            // Tremor na tela
            if (screenShaker != null)
            {
                screenShaker.Shake(-knockbackDirection.normalized * shakeIntensity);
            }

            StartCoroutine(InvulnerabilityCoroutine());

            if (playerData.currentHealth <= 0)
            {
                Die();
            }
        }

        IEnumerator InvulnerabilityCoroutine()
        {
            isInvulnerable = true;

            if (spriteRenderer != null)
            {
                float elapsedTime = 0f;
                while (elapsedTime < invulnerabilityTime)
                {
                    spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);
                    yield return new WaitForSeconds(0.1f);
                    spriteRenderer.color = Color.white;
                    yield return new WaitForSeconds(0.1f);
                    elapsedTime += 0.2f;
                }
                spriteRenderer.color = Color.white;
            }

            isInvulnerable = false;
        }

        public void Heal(int amount)
        {
            playerData.currentHealth += amount;
            if (playerData.currentHealth > playerData.maxHealth)
                playerData.currentHealth = playerData.maxHealth;

            playerData.healthBar.SetHealth(playerData.currentHealth);
            Debug.Log("Recuperei " + amount + " de vida. Vida atual: " + playerData.currentHealth);
        }

        public void Die()
        {
            if (hasDiedThisLife) return; // Evita chamadas múltiplas na mesma vida
            hasDiedThisLife = true; // Marca que o player morreu nesta vida

            Debug.Log("Player morreu! Notificando Game Over Manager e Player Death Handler...");
            OnPlayerDeath?.Invoke(); // Dispara o evento

            // Desativa componentes básicos do Player para o efeito de morte visual
            // O PlayerDeathHandler cuidará da "limpeza" mais abrangente
            this.enabled = false; // Desativa o DamageHandler para evitar mais eventos de dano

            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false; // Oculta o sprite do player
            }
            Collider2D playerCollider = GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                playerCollider.enabled = false; // Desativa colisões
            }
        }
    }
}