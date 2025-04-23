using UnityEngine;
using System; // Importante para usar o tipo Action para eventos

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;

    // Evento que será invocado quando a vida do chefe mudar
    public event Action<int, int> OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("Vida do Chefe: " + currentHealth + "/" + maxHealth);
        // Garante que o evento seja invocado com os valores iniciais
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        BossController bossController = GetComponent<BossController>();
        if (bossController != null && !bossController.isVulnerable)
        {
            Debug.Log("Chefe invulnerável, dano ignorado.");
            return;
        }

        currentHealth -= damageAmount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        Debug.Log("Chefe recebeu " + damageAmount + " de dano. Vida restante: " + currentHealth);

        // Invoca o evento OnHealthChanged sempre que a vida mudar
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Chefe derrotado!");
        // Adicione aqui a lógica para a derrota do chefe (animações, drops, etc.)
        Destroy(gameObject); // Por enquanto, apenas destrói o chefe
    }
}