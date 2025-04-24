using UnityEngine;
using Player; // Certifique-se de que este namespace esteja presente

public class Key : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Obtém o componente PlayerData do jogador
            PlayerData playerData = other.GetComponent<PlayerData>();
            if (playerData != null)
            {
                playerData.HasKey(); // Chama o método para indicar que uma chave foi coletada
                Destroy(gameObject); // Destrói a chave
            }
            else
            {
                Debug.LogError("O Player não possui o script PlayerData!");
            }
        }
    }
}