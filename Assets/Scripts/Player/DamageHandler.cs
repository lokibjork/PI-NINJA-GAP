using UnityEngine;
using System.Collections;

public class DamageHandler : MonoBehaviour
{
    public Player.PlayerData playerData;
    private bool isInvulnerable = false;
    public float invulnerabilityTime = 1.5f;
    public Rigidbody2D rb;

    void Start()
    {
        if (playerData == null)
        {
            playerData = GetComponent<Player.PlayerData>();
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        if (isInvulnerable) return; // Se estiver invulnerável, ignora o dano

        playerData.currentHealth -= damage;
        playerData.healthBar.SetHealth(playerData.currentHealth);
        
        // Aplica o knockback
        rb.linearVelocity = Vector2.zero; // Reseta a velocidade antes de aplicar
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        StartCoroutine(InvulnerabilityCoroutine());

        if (playerData.currentHealth <= 0)
        {
            
        }
    }

    IEnumerator InvulnerabilityCoroutine()
    {
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }
}