using UnityEngine;
using Player;

public class Dano : MonoBehaviour
{
    public int damage = 10;
    
    // Referência para a health bar do jogador, arraste pelo Inspector
    public HealthBar healthBar;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // Tenta pegar o PlayerData do jogador
            PlayerData playerData = collision.gameObject.GetComponent<PlayerData>();
            if (playerData != null)
            {
                // Aplica o dano
                playerData.TakeDamage(damage);
                // Atualiza a health bar com o valor atual da saúde do jogador
                if (healthBar != null)
                {
                    healthBar.SetHealth(playerData.currentHealth);
                }
            }
        }
    }
}