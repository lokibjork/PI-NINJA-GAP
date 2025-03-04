using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerData : MonoBehaviour
    {
        public int maxHealth = 5;
        public int currentHealth;
        public int dinheiro = 100;  // Quantidade de dinheiro inicial
        public int experience = 0;

        public HealthBar healthBar;

        void Start()
        {
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
            Debug.Log("Tomei " + damage + " de dano. Vida atual: " + currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(int amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            healthBar.SetHealth(currentHealth);
            Debug.Log("Recuperei " + amount + " de vida. Vida atual: " + currentHealth);
        }

        void Die()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}