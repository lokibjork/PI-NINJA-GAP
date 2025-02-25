using UnityEngine;

public class Dano : MonoBehaviour
{
    public int damage = 10;
    public float knockbackForce = 15f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DamageHandler damageHandler = collision.gameObject.GetComponent<DamageHandler>();
            if (damageHandler != null)
            {
                // Calcula a direção do knockback
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

                // Aplica o dano e o knockback
                damageHandler.TakeDamage(damage, knockbackDirection, knockbackForce);
            }
        }
    }
}