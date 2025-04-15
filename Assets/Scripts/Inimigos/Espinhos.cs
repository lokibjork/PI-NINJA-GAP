using UnityEngine;

public class Espinhos : DamageHandler
{
    DamageHandler DamageHandler;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Die();
        }
    }
}
