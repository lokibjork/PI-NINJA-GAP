using UnityEngine;
using Player; // Certifique-se de que este namespace esteja presente

public class Key : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Obt�m o componente PlayerData do jogador
            PlayerData playerData = other.GetComponent<PlayerData>();
            if (playerData != null)
            {
                playerData.HasKey(); // Chama o m�todo para indicar que uma chave foi coletada
                Destroy(gameObject); // Destr�i a chave
            }
            else
            {
                Debug.LogError("O Player n�o possui o script PlayerData!");
            }
        }
    }
}