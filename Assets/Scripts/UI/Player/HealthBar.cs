using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public PlayerData playerData;
    public TMP_Text playerHealthText; // Renomeei para deixar claro que � o texto

    public void SetMaxHealth(int health)
    {
        if (playerData != null)
        {
            playerData.maxHealth = health; // Atribui o valor num�rico da sa�de m�xima
            playerData.currentHealth = health; // Inicializa a vida atual com a m�xima
            slider.maxValue = health;
            slider.value = health;
            UpdateHealthText(health); // Atualiza o texto da sa�de inicial
        }
        else
        {
            Debug.LogError("PlayerData n�o est� atribu�do � HealthBar!");
        }
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        if (playerData != null)
        {
            playerData.currentHealth = health; // Atualiza a vida atual no PlayerData
            UpdateHealthText(health); // Atualiza o texto da sa�de
        }
    }

    // Fun��o para atualizar o texto da sa�de na UI
    private void UpdateHealthText(int currentHealth)
    {
        if (playerHealthText != null)
        {
            playerHealthText.text = currentHealth.ToString(); // Converte o valor da sa�de para string
        }
        else
        {
            Debug.LogError("Texto da vida do jogador (TMP) n�o est� atribu�do � HealthBar!");
        }
    }
}