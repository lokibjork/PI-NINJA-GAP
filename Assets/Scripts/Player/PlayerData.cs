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

            if (PlayerPrefs.HasKey("CheckpointX") && PlayerPrefs.HasKey("CheckpointY"))
            {
                float x = PlayerPrefs.GetFloat("CheckpointX");
                float y = PlayerPrefs.GetFloat("CheckpointY");
                transform.position = new Vector2(x, y);
                Debug.Log("Player voltou pro checkpoint salvo");
            }
        }
    }
}