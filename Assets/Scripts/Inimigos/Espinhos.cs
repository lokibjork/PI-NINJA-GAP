using UnityEngine;

public class Espinhos : EnemyBase
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        { 

        }
    }
}
