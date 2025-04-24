using UnityEngine;
using Player; // Certifique-se de que este namespace esteja presente

public class Door : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Obt�m o componente PlayerData do jogador
            PlayerData playerData = other.GetComponent<PlayerData>();
            if (playerData != null)
            {
                if (playerData.CanOpenDoor())
                {
                    Debug.Log("Porta aberta com uma chave.");
                    // Adicione aqui qualquer l�gica para abrir a porta
                    Destroy(gameObject); // Destr�i a porta
                    playerData.UseKey(); // Indica que uma chave foi usada (opcional)
                }
                else
                {
                    Debug.Log("Voc� precisa de uma chave para abrir esta porta.");
                    // Adicione aqui qualquer feedback visual ou sonoro para a porta trancada
                }
            }
            else
            {
                Debug.LogError("O Player n�o possui o script PlayerData!");
            }
        }
    }
}