using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private BossHealth bossHealth; // Referência ao script de saúde do Boss
    [SerializeField] private Image healthBarFill; // Referência à Image que representa o preenchimento da barra de vida

    void Start()
    {
        if (bossHealth == null)
        {
            Debug.LogError("BossHealth não atribuído ao BossHealthUI!");
            enabled = false;
            return;
        }

        if (healthBarFill == null)
        {
            Debug.LogError("Image da barra de vida não atribuída ao BossHealthUI!");
            enabled = false;
            return;
        }

        // Inicializa a barra de vida
        UpdateHealthBar(bossHealth.currentHealth, bossHealth.maxHealth);

        // Garante que a UI seja atualizada quando a vida do Boss mudar
        bossHealth.OnHealthChanged += UpdateHealthBar;
    }

    void OnDestroy()
    {
        // Remove o listener para evitar erros
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBarFill != null && maxHealth > 0)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
        else if (healthBarFill != null)
        {
            healthBarFill.fillAmount = 0f;
        }
    }
}