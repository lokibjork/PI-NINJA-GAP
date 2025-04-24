using UnityEngine;
using Player; // Certifique-se de que este namespace esteja presente

public class Door : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Obtém o componente PlayerData do jogador
            PlayerData playerData = other.GetComponent<PlayerData>();
            if (playerData != null)
            {
                if (playerData.CanOpenDoor())
                {
                    Debug.Log("Porta aberta com uma chave.");
                    // Adicione aqui qualquer lógica para abrir a porta
                    Destroy(gameObject); // Destrói a porta
                    playerData.UseKey(); // Indica que uma chave foi usada (opcional)
                }
                else
                {
                    Debug.Log("Você precisa de uma chave para abrir esta porta.");
                    // Adicione aqui qualquer feedback visual ou sonoro para a porta trancada
                }
            }
            else
            {
                Debug.LogError("O Player não possui o script PlayerData!");
            }
        }
    }
}