using Player;
using UnityEngine;

public class Espinhos : MonoBehaviour {
    public DamageHandler DamageHandler;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DamageHandler.TakeDamage(10, transform.position, 1f);
        }
    }
}
