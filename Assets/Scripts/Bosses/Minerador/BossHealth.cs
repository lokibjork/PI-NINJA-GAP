using UnityEngine;
using UnityEngine.Events; // Necess�rio para UnityEvent

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;

    // Evento disparado quando a vida do chefe muda
    public UnityEvent<int, int> OnHealthChanged; // (vida atual, vida m�xima)

    // Evento disparado quando o chefe morre
    public UnityEvent OnBossDeath;

    void Start()
    {
        currentHealth = maxHealth;
        // Garante que o evento OnHealthChanged seja inicializado se n�o foi no Inspector
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
            // Garante que a vida n�o seja menor que zero
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }
            // Dispara o evento de mudan�a de vida
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
            // Garante que a vida n�o exceda a vida m�xima
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            // Dispara o evento de mudan�a de vida
            OnHealthChanged.Invoke(currentHealth, maxHealth);
        }
    }

    private void Die()
    {
        // Dispara o evento de morte do chefe
        OnBossDeath.Invoke();
        // Adicione aqui qualquer l�gica adicional para a morte do chefe
        Debug.Log("Chefe derrotado!");
        Destroy(gameObject); // Ou qualquer outra forma de remover o chefe
    }
}