using UnityEngine;

public class VerticalPlatform : MonoBehaviour
{
    [Header("Pontos de Movimento")]
    public Transform pointA;
    public Transform pointB;

    [Header("Configurações de Movimento")]
    public float speed = 2f;
    public float delayAtPoints = 0.5f; // Tempo de espera em segundos nos pontos

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
}