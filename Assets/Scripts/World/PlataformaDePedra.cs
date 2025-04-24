using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bala"))
        {
            // Plataforma atingida por uma bala
            Break();

            // Destrói a bala
            Destroy(collision.gameObject);
        }
    }

    private void Break()
    {
        // Adicione aqui qualquer efeito visual ou sonoro de quebra, se desejar.

        // Destrói o GameObject da plataforma
        Destroy(gameObject);
    }
}