using UnityEngine;
using System.Collections;

public class DamageHandler : MonoBehaviour
{
    public Player.PlayerData playerData;
    private bool isInvulnerable = false;
    public float invulnerabilityTime = 0.5f;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer; // 🔵 Para o efeito de piscar

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
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        if (isInvulnerable) return; // Se estiver invulnerável, ignora o dano

        playerData.currentHealth -= damage;
        playerData.healthBar.SetHealth(playerData.currentHealth);

        // Aplica o knockback
        rb.linearVelocity = Vector2.zero; // 🔄 Corrige problemas com velocidade acumulada
        rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);

        StartCoroutine(InvulnerabilityCoroutine());

        if (playerData.currentHealth <= 0)
        {
            // 🔴 Adicione lógica de morte aqui, se precisar
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
}