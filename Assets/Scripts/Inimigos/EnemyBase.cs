using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int maxHealth = 2;
    public int currentHealth;
    public float knockbackForce = 5f;
    public float flashTime = 0.1f;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.1f;
    [SerializeField] public AudioClip[] _hitClip;
    [SerializeField] public AudioClip _killClip;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    [SerializeField] private Material flashMaterial; // Material com o shader de brilho
    protected Rigidbody2D rb;
    private ScreenShaker screenShaker;

    // NOVOS CAMPOS PARA OS EFEITOS DE MORTE DO INIMIGO
    [Header("Efeitos de Morte do Inimigo")]
    [Tooltip("Prefab do pedaço de tomate (com Rigidbody2D).")]
    public GameObject tomatoChunkPrefab;
    [Tooltip("Prefab das partículas de sangue.")]
    public GameObject bloodParticlePrefab;
    public int numberOfTomatoChunks = 5; // Menos pedaços para inimigos menores
    public float explosionForce = 3f; // Força menor para inimigos
    [Tooltip("Tempo em segundos que os efeitos de morte (partículas/pedaços) duram antes de serem destruídos.")]
    public float deathEffectDuration = 1.5f; // Duração dos efeitos

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        screenShaker = FindObjectOfType<ScreenShaker>(); // Procure pelo ScreenShaker na cena

        if (screenShaker == null)
        {
            Debug.LogWarning("ScreenShaker não encontrado na cena! Certifique-se de ter um em alguma parte da hierarquia.");
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
            Debug.LogError("SpriteRenderer não encontrado no inimigo!");
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        currentHealth -= damage;

        // Aplica knockback
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Zera a velocidade antes de aplicar nova força
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
        SoundManagerSO.PlaySoundFXClips(_hitClip, transform.position, 1); // Descomente se SoundManagerSO for acessível

        // Feedback de dano (flash e shake)
        StartCoroutine(DamageFeedback());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual IEnumerator DamageFeedback()
    {
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
            // Fallback para o método antigo, caso o shader não esteja configurado
            StartCoroutine(FlashWhiteFallback());
        }

        // Screen Shake
        if (screenShaker != null)
        {
            // Ajuste a direção do shake se necessário, ou use Vector3.zero para um shake global
            screenShaker.Shake(transform.position.normalized * shakeIntensity);
        }
    }

    protected virtual IEnumerator FlashWhiteFallback()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color; // Captura a cor original do sprite
            spriteRenderer.color = Color.red; // Ou a cor de flash desejada
            yield return new WaitForSeconds(flashTime);
            spriteRenderer.color = originalColor; // Retorna à cor original
        }
        yield return null;
    }

    protected virtual void Die()
    {
        SoundManagerSO.PlaySoundFXClip(_killClip, transform.position, 1); // Descomente se SoundManagerSO for acessível

        // --- INÍCIO DOS EFEITOS DE MORTE DO INIMIGO ---
        // Instancia os pedaços de tomate (ou outro tipo de "pedaço")
        for (int i = 0; i < numberOfTomatoChunks; i++)
        {
            if (tomatoChunkPrefab != null)
            {
                GameObject chunk = Instantiate(tomatoChunkPrefab, transform.position, Random.rotation);
                Rigidbody2D rbChunk = chunk.GetComponent<Rigidbody2D>();
                if (rbChunk != null)
                {
                    Vector2 randomDirection = Random.insideUnitCircle.normalized;
                    rbChunk.AddForce(randomDirection * explosionForce, ForceMode2D.Impulse);
                }
                // Destroi os pedaços de tomate após um tempo para não lotar a cena
                Destroy(chunk, deathEffectDuration);
            }
        }

        // Instancia as partículas de sangue
        if (bloodParticlePrefab != null)
        {
            GameObject particles = Instantiate(bloodParticlePrefab, transform.position, Quaternion.identity);
            // Destroi as partículas de sangue após um tempo
            Destroy(particles, deathEffectDuration);
        }
        // --- FIM DOS EFEITOS DE MORTE DO INIMIGO ---

        // Destroi o próprio inimigo
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