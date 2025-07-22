using UnityEngine;

public class VerticalPlatform : MonoBehaviour
{
    [Header("Pontos de Movimento")]
    public Transform pointA;
    public Transform pointB;

    [Header("Configurações de Movimento")]
    public float speed = 2f;
    public float delayAtPoints = 0.5f; // Tempo de espera em segundos nos pontos

    // Adicionado: Para gerenciar o player como filho
    private Transform currentPlayerOnPlatform;
    private BoxCollider2D platformCollider; // Referência ao collider da plataforma

    [Header("Detecção do Jogador")]
    [Tooltip("Distância vertical do topo da plataforma para verificar se o jogador está acima.")]
    public float playerDetectionOffset = 0.1f;
    [Tooltip("Layer do jogador para a detecção.")]
    public LayerMask playerLayer;

    private Vector3 targetPosition;
    private float timer;
    private bool movingTowardsB = true;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("Os pontos A e B da plataforma não foram atribuídos! Por favor, configure-os no Inspector.");
            enabled = false; // Desativa o script para evitar erros
            return;
        }

        transform.position = pointA.position; // Inicia na posição do ponto A
        targetPosition = pointB.position;

        // Pega a referência do BoxCollider2D da plataforma
        platformCollider = GetComponent<BoxCollider2D>();
        if (platformCollider == null)
        {
            Debug.LogError("VerticalPlatform requer um BoxCollider2D no mesmo GameObject!");
            enabled = false;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= delayAtPoints)
        {
            // Move a plataforma em direção ao ponto alvo
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Verifica se chegou ao ponto alvo
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                // Inverte a direção do movimento
                movingTowardsB = !movingTowardsB;
                targetPosition = movingTowardsB ? pointB.position : pointA.position;
                timer = 0f; // Reinicia o timer para o delay no próximo ponto
            }
        }

        // NOVO: Verifica se o jogador ainda está em cima da plataforma a cada frame
        // Isso é mais confiável do que apenas confiar no OnCollisionExit2D
        CheckAndReleasePlayer();
    }

    // Desenha gizmos no editor para visualizar os pontos de movimento
    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pointA.position, 0.5f);
            Gizmos.DrawWireSphere(pointB.position, 0.5f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Certifica-se de que é o jogador e que ele está "em cima" da plataforma
        // Usamos platformCollider.size.y / 2 para pegar o topo do collider
        if (collision.gameObject.CompareTag("Player") && platformCollider != null)
        {
            // Calcula a posição do topo da plataforma
            float platformTop = transform.position.y + platformCollider.offset.y + platformCollider.size.y / 2f;

            // Verifica se o ponto de contato está acima do meio do collider da plataforma
            // E se a velocidade vertical do player é negativa ou quase zero (indicando que ele está pousando)
            if (collision.contacts.Length > 0 &&
                collision.contacts[0].point.y > (transform.position.y + platformCollider.offset.y - platformCollider.size.y / 2f + 0.1f)) // Apenas um pouco acima da base
            {
                // Verifica a velocidade do player no eixo Y, para evitar que ele seja filho ao colidir lateralmente
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null && playerRb.linearVelocity.y <= 0.1f) // Se o player está caindo ou parado em Y
                {
                    currentPlayerOnPlatform = collision.transform;
                    currentPlayerOnPlatform.SetParent(transform);
                    Debug.Log($"Player se tornou filho da plataforma: {gameObject.name}");
                }
            }
        }
    }

    // NOVO: Método para verificar e liberar o player
    private void CheckAndReleasePlayer()
    {
        if (currentPlayerOnPlatform == null) return; // Não há player para liberar

        // Calcula a posição do topo do BoxCollider da plataforma
        Vector2 platformTopCenter = (Vector2)transform.position + platformCollider.offset + new Vector2(0, platformCollider.size.y / 2f);

        // Crie uma pequena "caixa" (OverlapBox) no topo da plataforma para verificar o jogador
        // Aumente o tamanho da caixa ligeiramente para cobrir toda a largura da plataforma
        Vector2 checkSize = new Vector2(platformCollider.size.x * transform.localScale.x * 0.9f, 0.1f); // Largura quase total, altura pequena
        Vector2 checkPosition = platformTopCenter + new Vector2(0, playerDetectionOffset); // Um pouco acima do topo da plataforma

        // Realiza a verificação de colisão
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(checkPosition, checkSize, 0f, playerLayer);

        bool playerIsStillOnPlatform = false;
        foreach (Collider2D hit in hitColliders)
        {
            if (hit.transform == currentPlayerOnPlatform)
            {
                playerIsStillOnPlatform = true;
                break;
            }
        }

        if (!playerIsStillOnPlatform)
        {
            // Se o jogador não está mais na área de detecção, desvincula
            if (currentPlayerOnPlatform != null) // Garante que o player ainda exista
            {
                currentPlayerOnPlatform.SetParent(null);
                Debug.Log($"Player desvinculado da plataforma: {gameObject.name}");
                currentPlayerOnPlatform = null;
            }
        }
    }

    // Opcional: Se quiser manter o OnCollisionExit2D como um fallback rápido
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform == currentPlayerOnPlatform)
        {
            // Ainda chamamos isso, mas a verificação no Update é a principal
            currentPlayerOnPlatform.SetParent(null);
            Debug.Log($"Player desvinculado por OnCollisionExit2D (fallback): {gameObject.name}");
            currentPlayerOnPlatform = null;
        }
    }
}