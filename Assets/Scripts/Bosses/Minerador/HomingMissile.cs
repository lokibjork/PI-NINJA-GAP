using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 200f;
    private Transform target;
    private Rigidbody2D rb;
    private int lockOnTimer = 60; // Tempo em frames antes de travar no alvo inicial
    private bool isLockedOn = false;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        isLockedOn = true;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Inicialmente, o alvo é o jogador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogError("Jogador não encontrado!");
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector2 direction = (Vector2)target.position - rb.position;
            direction.Normalize();

            // Calcula o ângulo para o alvo
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Rotaciona o míssil em direção ao alvo
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle - 90f); // Subtrai 90 porque a frente do sprite pode não ser o "up" local
            rb.rotation = Mathf.MoveTowardsAngle(rb.rotation, targetRotation.eulerAngles.z, rotationSpeed * Time.fixedDeltaTime);

            // Move o míssil para frente (agora na direção visual correta)
            rb.linearVelocity = transform.up * speed;
        }
        else
        {
            // Se o alvo for destruído, o míssil continua em sua direção atual
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DamageHandler playerDamageHandler = other.GetComponent<DamageHandler>();
            if (playerDamageHandler != null)
            {
                Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
                playerDamageHandler.TakeDamage(1, knockbackDirection, 5f);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Boss"))
        {
            BossHealth bossHealth = other.GetComponent<BossHealth>();
            if (bossHealth != null && isLockedOn)
            {
                bossHealth.TakeDamage(1);
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("Bala"))
        {
            // Redireciona para o boss ao colidir com um tiro do jogador
            GameObject boss = GameObject.FindGameObjectWithTag("Boss");
            if (boss != null)
            {
                SetTarget(boss.transform);
                // Desativa a colisão com outros projéteis do jogador para evitar loops
                Collider2D playerProjectileCollider = other.GetComponent<Collider2D>();
                if (playerProjectileCollider != null)
                {
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerProjectileCollider);
                }
            }
            // Não destrói o míssil ao colidir com o tiro do jogador
        }
        // Adicione outras colisões conforme necessário
    }
}