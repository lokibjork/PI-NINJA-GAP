using UnityEngine;
using Player; // Certifique-se de ter este namespace se DamageHandler estiver lá

public class Drone : EnemyBase
{
    public float moveSpeed = 3f;
    public float hoverHeight = 2f;
    public float circleRadius = 1.5f;
    public float circleSpeed = 2f;
    public int damage = 1;
    public float initialAngleOffset = 0f;

    private Transform player;
    private float angle = 0f;
    private bool playerDetected = false;
    public AudioClip detectClip;

    [Tooltip("Ajuste se o drone deve ser virado no eixo Y (normalmente -1 para flip horizontal).")]
    public float flipYScale = 1f; // Use 1f para manter a escala Y, -1f para inverter

    override protected void Start()
    {
        base.Start(); // Chama o Start da classe base (EnemyBase)
        maxHealth = 4; // Pode definir valores específicos para o Drone
        currentHealth = maxHealth;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        angle = initialAngleOffset;
    }

    void Update()
    {
        // Se o jogador não foi detectado ou é nulo, o drone não se move nem vira
        if (!playerDetected || player == null) return;

        // Atualiza o ângulo para o movimento circular
        angle += circleSpeed * Time.deltaTime;

        // Calcula a posição alvo para o movimento circular ao redor do jogador
        Vector2 targetPosition = new Vector2(
            player.position.x + Mathf.Cos(angle) * circleRadius,
            player.position.y + hoverHeight + Mathf.Sin(angle) * circleRadius
        );

        // Calcula a direção para o alvo
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed; // Aplica a velocidade

        // NOVO: Chama a função Flip da classe base para virar o drone
        // O `direction.x` indica para qual lado o drone está se movendo horizontalmente.
        Flip(direction.x);
    }

    // Esse Trigger é o trigger separado (GameObject filho com tag "IgnoreBullet")
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!playerDetected && collision.CompareTag("Player"))
        {
            playerDetected = true;
            SoundManagerSO.PlaySoundFXClip(detectClip, transform.position, 1f); 
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DamageHandler damageHandler = collision.gameObject.GetComponent<DamageHandler>();
            if (damageHandler != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                damageHandler.TakeDamage(damage, knockbackDirection, 10f);
            }
        }
    }

    // Override do método Die da EnemyBase para adicionar lógica específica do Drone (opcional)
    protected override void Die()
    {
        base.Die(); // Chama a lógica de morte da EnemyBase (pedaços, sangue, destroy do objeto)
        // Adicione qualquer lógica adicional para o Drone ao morrer aqui, se necessário
        // Por exemplo, uma explosão de partículas maior, ou um som específico de destruição de drone.
    }
}