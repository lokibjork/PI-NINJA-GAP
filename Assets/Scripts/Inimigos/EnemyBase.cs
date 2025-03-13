using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int maxHealth = 2;
    protected int currentHealth;
    public float knockbackForce = 5f;
    public float flashTime = 0.1f;
    private SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb;
    [SerializeField] public AudioClip[] _hitClip;
    [SerializeField] public AudioClip _killClip;// Protegido para ser acessado nas classes filhas

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
        SoundManagerSO.PlaySoundFXClips(_hitClip, transform.position, 1);

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
        SoundManagerSO.PlaySoundFXClip(_killClip, transform.position, 1);
        Destroy(gameObject);
    }

    protected void Flip(float directionX)
    {
        if (directionX > 0 && transform.localScale.x < 0 || directionX < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}