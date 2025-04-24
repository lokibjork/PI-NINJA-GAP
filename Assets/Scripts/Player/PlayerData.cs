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

        private bool hasAKey = false; // Booleano para indicar se o jogador possui alguma chave
        // Ou, voc� pode usar um contador:
        // private int keyCount = 0;

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

        private void OnApplicationQuit()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("PlayerPrefs deletados ao fechar o jogo.");
        }

        // M�todo para indicar que uma chave foi coletada
        public void HasKey()
        {
            hasAKey = true;
            Debug.Log("Chave coletada!");
            // Se usar um contador:
            // keyCount++;
            // Debug.Log("Chave coletada. Total de chaves: " + keyCount);
            // Adicione aqui qualquer feedback visual ou sonoro
        }

        // M�todo para verificar se o jogador possui alguma chave
        public bool CanOpenDoor()
        {
            return hasAKey;
            // Se usar um contador:
            // return keyCount > 0;
        }

        // M�todo para "usar" uma chave ao abrir uma porta (opcional, se a chave for consumida)
        public void UseKey()
        {
            hasAKey = false;
            Debug.Log("Chave usada para abrir a porta.");
            // Se usar um contador:
            // if (keyCount > 0)
            // {
            //     keyCount--;
            //     Debug.Log("Chave usada. Total de chaves restantes: " + keyCount);
            // }
        }
    }
}