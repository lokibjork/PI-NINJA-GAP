using Player;
using UnityEngine;

public class LaserDamage : MonoBehaviour
{
    public int damageAmount = 1;
    public float damageRate = 0.1f; // Intervalo em segundos entre os danos (para dano contínuo)
    public string damageTargetTag = "Player"; // Ajuste para a tag do seu jogador ou outros alvos
    public float knockbackForce = 2f; // Força do knockback do laser
    public float laserShakeIntensity = 0.1f; // Intensidade do shake do laser
    private float timer = 0f;
    private Collider2D lastHitTarget = null;
    private ScreenShaker laserScreenShaker;
    public float destroyTime;// Referência ao ScreenShaker local (se houver) ou global

    void Start()
    {
        // Tenta encontrar um ScreenShaker neste GameObject ou em seus pais
        laserScreenShaker = GetComponentInParent<ScreenShaker>();
        if (laserScreenShaker == null)
        {
            // Se não encontrar localmente, tenta encontrar um global na cena
            laserScreenShaker = FindObjectOfType<ScreenShaker>();
            if (laserScreenShaker == null)
            {
                Debug.LogWarning("ScreenShaker não encontrado na cena para o laser!");
            }
        }

        Destroy(this.gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(damageTargetTag))
        {
            ApplyDamage(other.gameObject, other);
            lastHitTarget = other;
            timer = 0f;
            StartLaserShake(); // Inicia o shake quando atinge um alvo
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(damageTargetTag) && damageRate > 0 && other == lastHitTarget)
        {
            timer += Time.deltaTime;
            if (timer >= damageRate)
            {
                ApplyDamage(other.gameObject, other);
                timer -= damageRate;
                if (laserScreenShaker != null)
                {
                    // Gera um impulso de shake na direção do laser
                    laserScreenShaker.Shake(transform.up * laserShakeIntensity);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(damageTargetTag) && other == lastHitTarget)
        {
            lastHitTarget = null;
            StopLaserShake(); // Para o shake quando o alvo sai do laser
        }
    }

    private void ApplyDamage(GameObject target, Collider2D hitCollider)
    {
        DamageHandler damageHandler = target.GetComponent<DamageHandler>();

        if (damageHandler != null)
        {
            Vector2 knockbackDirection = (hitCollider.bounds.center - transform.position).normalized;
            damageHandler.TakeDamage(damageAmount, knockbackDirection, knockbackForce);
        }
    }

    private void StartLaserShake()
    {
        // Usando GenerateImpulse para um shake inicial quando o laser atinge
        if (laserScreenShaker != null)
        {
            laserScreenShaker.Shake(transform.up * laserShakeIntensity);
        }
    }

    private void StopLaserShake()
    {
        // No seu ScreenShaker atual, o shake é um impulso. Não há um "parar" contínuo.
        // Podemos simplesmente parar de chamar Shake().
    }
}