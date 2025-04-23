using Unity.Cinemachine;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 200f; // Não será usado para rotação direta, mas pode ser útil para outras coisas
    private Transform target;
    private Rigidbody2D rb;
    public int lockOnTimer = 60; // Tempo em frames antes de travar no alvo inicial
    private bool isLockedOn = false;
    private Transform initialTarget; // Guarda o alvo inicial (jogador)
    private ScreenShaker screenShaker;

    [Header("Efeitos Visuais ao Impacto")]
    public float impactShakeIntensityPlayer = 1f; // Ajuste a força do shake
    public float impactShakeIntensityBoss = 0.75f; // Ajuste a força do shake

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        isLockedOn = true;
    }

    void Start()
    {
        screenShaker = GetComponent<ScreenShaker>();
        rb = GetComponent<Rigidbody2D>();
        // Inicialmente, o alvo é o jogador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            initialTarget = player.transform;
            target = initialTarget; // Inicialmente, o alvo é o jogador
        }
        else
        {
            Debug.LogError("Jogador não encontrado!");
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (!isLockedOn)
        {
            if (lockOnTimer > 0)
            {
                lockOnTimer--;
                if (initialTarget != null)
                {
                    target = initialTarget; // Continua seguindo o alvo inicial
                }
            }
            else
            {
                isLockedOn = true; // Trava no alvo inicial após o timer
            }
        }

        if (target != null)
        {
            // Obtém a posição do alvo
            Vector3 targetPos = target.position;
            // Calcula a direção PARA o alvo
            Vector2 directionToTarget = (targetPos - transform.position).normalized;

            // Calcula o ângulo para o alvo
            float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

            // Define a rotação do míssil para apontar para o alvo (sem a subtração de 90 agora)
            rb.rotation = targetAngle;

            // Move o míssil DIRETAMENTE NA DIREÇÃO DO ALVO
            rb.linearVelocity = directionToTarget * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // Para de se mover se o alvo for perdido
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ScreenShaker shaker = FindObjectOfType<ScreenShaker>(); // Encontra uma instância do ScreenShaker na cena

        if (other.CompareTag("Player"))
        {
            DamageHandler playerDamageHandler = other.GetComponent<DamageHandler>();

            if (playerDamageHandler != null)

            {

                Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;

                playerDamageHandler.TakeDamage(1, knockbackDirection, 5f);



                // Chama o screen shake ao atingir o jogador

                if (shaker != null)

                {

                    screenShaker.Shake(other.transform.position - transform.position); // Passa a direção do impacto como impulso

                }

            }

            Destroy(this.gameObject);
        }
        else if (other.CompareTag("Boss"))
        {
            BossHealth bossHealth = other.GetComponent<BossHealth>();
            if (bossHealth != null && isLockedOn)
            {
                bossHealth.TakeDamage(1); // Aplica dano ao chefe

                // Chama o screen shake ao atingir o chefe
                if (shaker != null)
                {
                    shaker.Shake(other.transform.position - transform.position);
                }
                Destroy(gameObject); // Destrói o míssil ao atingir o chefe
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

                // Chama o screen shake ao atingir o projétil do jogador (antes do redirecionamento)
                if (shaker != null)
                {
                    screenShaker.Shake(other.transform.position - transform.position); // Passa a direção do impacto
                }

                speed = 10f;
            }
            // Não destrói o míssil ao colidir com o tiro do jogador
        }
        // Adicione outras colisões conforme necessário
    }
}