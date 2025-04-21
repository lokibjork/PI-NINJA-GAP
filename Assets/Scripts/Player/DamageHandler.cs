using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DamageHandler : MonoBehaviour
{
    public Player.PlayerData playerData;
    private bool isInvulnerable = false;
    public float invulnerabilityTime = 0.2f;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public AudioClip _playerHit;
    public ScreenShaker screenShaker;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.1f;

    void Start()
    {
        if (playerData == null)
        {
            playerData = GetComponent<Player.PlayerData>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        screenShaker = GetComponent<ScreenShaker>();
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        if (isInvulnerable) return; // Se estiver invulnerável, ignora o dano

        playerData.currentHealth -= damage;
        playerData.healthBar.SetHealth(playerData.currentHealth);
        SoundManagerSO.PlaySoundFXClip(_playerHit, transform.position, 1f);

        // Aplica o knockback
        rb.linearVelocity = Vector2.zero; // 🔄 Corrige problemas com velocidade acumulada
        rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);

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

        // 🔥 Efeito de piscar transparente
        float elapsedTime = 0f;
        while (elapsedTime < invulnerabilityTime)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f); // Transparente
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white; // Normal
            yield return new WaitForSeconds(0.1f);
            elapsedTime += 0.2f;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}