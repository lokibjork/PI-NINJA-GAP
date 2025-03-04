using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int maxHealth = 2;
    protected int currentHealth;
    public float knockbackForce = 5f;
    public float flashTime = 0.1f;
    private SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb; // Protegido para ser acessado nas classes filhas

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        currentHealth -= damage;

        // Aplica knockback
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        // Pisca branco
        StartCoroutine(FlashWhite());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(flashTime);
        spriteRenderer.color = Color.red;
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}