using UnityEngine;
using UnityEngine.Events; // Necessário para UnityEvent

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;

    // Evento disparado quando a vida do chefe muda
    public UnityEvent<int, int> OnHealthChanged; // (vida atual, vida máxima)

    // Evento disparado quando o chefe morre
    public UnityEvent OnBossDeath;

    void Start()
    {
        currentHealth = maxHealth;
        // Garante que o evento OnHealthChanged seja inicializado se não foi no Inspector
        if (OnHealthChanged == null)
        {
            OnHealthChanged = new UnityEvent<int, int>();
        }
        if (OnBossDeath == null)
        {
            OnBossDeath = new UnityEvent();
        }
        // Dispara o evento de vida inicial
        OnHealthChanged.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        if (damageAmount > 0)
        {
            currentHealth -= damageAmount;
            // Garante que a vida não seja menor que zero
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }
            // Dispara o evento de mudança de vida
            OnHealthChanged.Invoke(currentHealth, maxHealth);

            // Verifica se o chefe morreu
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void Heal(int healAmount)
    {
        if (healAmount > 0)
        {
            currentHealth += healAmount;
            // Garante que a vida não exceda a vida máxima
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            // Dispara o evento de mudança de vida
            OnHealthChanged.Invoke(currentHealth, maxHealth);
        }
    }

    private void Die()
    {
        // Dispara o evento de morte do chefe
        OnBossDeath.Invoke();
        // Adicione aqui qualquer lógica adicional para a morte do chefe
        Debug.Log("Chefe derrotado!");
        Destroy(gameObject); // Ou qualquer outra forma de remover o chefe
    }
}