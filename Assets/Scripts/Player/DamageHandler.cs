using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Certifique-se que esta linha está presente

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
        public AudioClip _deathSound;
        public ScreenShaker screenShaker;
        public float shakeIntensity = 0.1f;
        public float shakeDuration = 0.1f;

        // Adicionado: Deleção e Desativação na Morte
        [Header("Deleção e Desativação na Morte")]
        [Tooltip("Array de GameObjects para deletar quando o jogador morrer (ex: filhos do player, armas, etc.).")]
        public GameObject[] objectsToDeleteOnDeath;
        [Tooltip("Array de scripts para desativar quando o jogador morrer (ex: PlayerMovement, PlayerShooting).")]
        public MonoBehaviour[] scriptsToDisableOnDeath;

        // Campos para os efeitos de morte
        [Header("Efeitos de Morte do Jogador")]
        [Tooltip("Prefab do pedaço de tomate (com Rigidbody2D).")]
        public GameObject tomatoChunkPrefab;
        [Tooltip("Prefab das partículas de sangue.")]
        public GameObject bloodParticlePrefab;
        public int numberOfTomatoChunks = 15;
        public float explosionForce = 5f;
        [Tooltip("Tempo em segundos que os efeitos de morte (partículas/pedaços) duram antes de serem destruídos. Se 0 ou negativo, ficam permanentes.")]
        public float deathEffectDuration = 2f;

        // Evento estático para notificar outros scripts sobre a morte do jogador
        public static event System.Action OnPlayerDeath;

        private bool hasDiedThisLife = false; // Flag para controlar se o player já morreu nesta "vida"

        void Awake()
        {
            // Resetar o estado ao carregar a cena
            hasDiedThisLife = false;
            isInvulnerable = false;
            this.enabled = true; // Garante que o DamageHandler esteja ativo no início

            // Inicializações de referências (melhor em Awake para evitar nulls no Start)
            if (playerData == null) playerData = GetComponent<PlayerData>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            // ScreenShaker geralmente é um Singleton ou global, então FindObjectOfType é mais comum
            if (screenShaker == null) screenShaker = FindObjectOfType<ScreenShaker>();


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
            if (isInvulnerable || hasDiedThisLife) return;

            playerData.currentHealth -= damage;
            playerData.healthBar.SetHealth(playerData.currentHealth);

            if (_playerHit != null)
            {
                // Certifique-se que SoundManagerSO é acessível e está funcionando
                SoundManagerSO.PlaySoundFXClip(_playerHit, transform.position, 1f); 
            }

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
            }

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
            if (hasDiedThisLife) return;
            hasDiedThisLife = true;

            Debug.Log("Player morreu! Iniciando efeitos e desativações.");

            SoundManagerSO.PlaySoundFXClip(_deathSound, transform.position, 1);

            // --- INÍCIO DA LÓGICA DE MORTE ---

            // 1. Deletar objetos listados
            if (objectsToDeleteOnDeath != null)
            {
                foreach (GameObject objToDelete in objectsToDeleteOnDeath)
                {
                    if (objToDelete != null)
                    {
                        Destroy(objToDelete);
                        Debug.Log($"Objeto deletado na morte do player: {objToDelete.name}");
                    }
                }
            }

            // 2. Desativar scripts listados
            if (scriptsToDisableOnDeath != null)
            {
                foreach (MonoBehaviour script in scriptsToDisableOnDeath)
                {
                    if (script != null && script.enabled) // Verifica se o script existe e está ativo antes de desativar
                    {
                        script.enabled = false;
                        Debug.Log($"Script desativado na morte do player: {script.GetType().Name}");
                    }
                }
            }

            // 3. Desativar o próprio DamageHandler e componentes visuais/físicos do Player
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
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // Zera a velocidade do player
                rb.bodyType = RigidbodyType2D.Static; // Torna estático para que não se mova ou caia
            }

            // 4. Efeitos de explosão (tomate e sangue)
            for (int i = 0; i < numberOfTomatoChunks; i++)
            {
                if (tomatoChunkPrefab != null)
                {
                    GameObject tomatoChunk = Instantiate(tomatoChunkPrefab, transform.position, Random.rotation);
                    Rigidbody2D rbChunk = tomatoChunk.GetComponent<Rigidbody2D>();
                    if (rbChunk != null)
                    {
                        Vector2 randomDirection = Random.insideUnitCircle.normalized;
                        rbChunk.AddForce(randomDirection * explosionForce, ForceMode2D.Impulse);
                    }
                    // Destroi o pedaço de tomate se deathEffectDuration for positivo
                    if (deathEffectDuration > 0)
                    {
                        Destroy(tomatoChunk, deathEffectDuration);
                    }
                }
            }

            if (bloodParticlePrefab != null)
            {
                GameObject bloodParticles = Instantiate(bloodParticlePrefab, transform.position, Quaternion.identity);
                // Destroi as partículas de sangue se deathEffectDuration for positivo
                if (deathEffectDuration > 0)
                {
                    Destroy(bloodParticles, deathEffectDuration);
                }
            }

            // 5. Dispara o evento de morte do jogador
            OnPlayerDeath?.Invoke();

            // --- FIM DA LÓGICA DE MORTE ---
        }
    }
}