using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int maxHealth = 2;
    public int currentHealth;
    public float knockbackForce = 5f;
    public float flashTime = 0.1f;
    public float timeStopDuration = 0.05f;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.1f;
    [SerializeField] public AudioClip[] _hitClip;
    [SerializeField] public AudioClip _killClip;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    [SerializeField] private Material flashMaterial; // Material com o shader de brilho
    protected Rigidbody2D rb;
    private ScreenShaker screenShaker;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        screenShaker = GetComponent<ScreenShaker>();

        if (screenShaker == null)
        {
            Debug.LogError("ScreenShaker n�o encontrado na cena!");
        }
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
        }
        else
        {
            Debug.LogError("SpriteRenderer n�o encontrado no inimigo!");
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        currentHealth -= damage;

        // Aplica knockback
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        SoundManagerSO.PlaySoundFXClips(_hitClip, transform.position, 1);

        // Feedback de dano
        StartCoroutine(DamageFeedback());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual IEnumerator DamageFeedback()
    {
        // Time Stop
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(timeStopDuration);
        Time.timeScale = 1f;

        // Flash Branco (usando shader)
        if (spriteRenderer != null && flashMaterial != null)
        {
            spriteRenderer.material = flashMaterial;
            flashMaterial.SetFloat("_FlashAmount", 1f);
            yield return new WaitForSeconds(flashTime);
            flashMaterial.SetFloat("_FlashAmount", 0f);
            spriteRenderer.material = originalMaterial;
        }
        else
        {
            // Fallback para o m�todo antigo, caso o shader n�o esteja configurado
            StartCoroutine(FlashWhiteFallback());
        }

        // Screen Shake
        if (screenShaker != null)
        {
            screenShaker.Shake(transform.position.normalized * shakeIntensity);
        }
    }

    protected virtual IEnumerator FlashWhiteFallback()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashTime);
            spriteRenderer.color = Color.red; // Ou a cor original do inimigo
        }
        yield return null;
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