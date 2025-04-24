using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public PlayerData playerData;
    public TMP_Text playerHealthText; // Renomeei para deixar claro que é o texto

    public void SetMaxHealth(int health)
    {
        if (playerData != null)
        {
            playerData.maxHealth = health; // Atribui o valor numérico da saúde máxima
            playerData.currentHealth = health; // Inicializa a vida atual com a máxima
            slider.maxValue = health;
            slider.value = health;
            UpdateHealthText(health); // Atualiza o texto da saúde inicial
        }
        else
        {
            Debug.LogError("PlayerData não está atribuído à HealthBar!");
        }
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        if (playerData != null)
        {
            playerData.currentHealth = health; // Atualiza a vida atual no PlayerData
            UpdateHealthText(health); // Atualiza o texto da saúde
        }
    }

    // Função para atualizar o texto da saúde na UI
    private void UpdateHealthText(int currentHealth)
    {
        if (playerHealthText != null)
        {
            playerHealthText.text = currentHealth.ToString(); // Converte o valor da saúde para string
        }
        else
        {
            Debug.LogError("Texto da vida do jogador (TMP) não está atribuído à HealthBar!");
        }
    }
}