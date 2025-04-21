using UnityEngine;

public class EjetarCapsula : MonoBehaviour
{
    [Header("Configurações da Cápsula")]
    public GameObject shellPrefab; // Prefab da cápsula da bala
    public Transform shellEjectionPoint; // Ponto de onde a cápsula será ejetada
    public float ejectionForce = 10f; // Força de ejeção da cápsula
    public float torqueForce = 2f; // Força de rotação da cápsula
    public float lifeTime = 2f; // Tempo em segundos antes da cápsula ser destruída (opcional)
    public GameObject smokePrefab;
    public Transform smokePoint;

    [Header("Configurações de Colisão")]
    public string playerLayerName = "Player"; // Nome da layer do jogador
    private int playerLayer;

    void Start()
    {
        playerLayer = LayerMask.NameToLayer(playerLayerName);
        if (playerLayer == -1)
        {
            Debug.LogError("Layer '" + playerLayerName + "' não encontrada. Certifique-se de que a layer do jogador está configurada corretamente.");
            enabled = false;
        }
    }

    public void Ejetar()
    {
        if (shellPrefab != null && shellEjectionPoint != null)
        {
            // Instancia a cápsula
            GameObject shell = Instantiate(shellPrefab, shellEjectionPoint.position, shellEjectionPoint.rotation);

            // Obtém o componente Rigidbody2D da cápsula (se houver)
            Rigidbody2D shellRb = shell.GetComponent<Rigidbody2D>();
            Collider2D shellCollider = shell.GetComponent<Collider2D>();
            if (shellRb != null && shellCollider != null)
            {
                // Calcula a direção de ejeção baseada na rotação do ponto de ejeção
                Vector2 ejectionDirection = shellEjectionPoint.up;

                // Aplica uma força para ejetar a cápsula
                shellRb.AddForce(ejectionDirection * ejectionForce, ForceMode2D.Impulse);

                // Aplica um torque (rotação) aleatório
                shellRb.AddTorque(Random.Range(-torqueForce, torqueForce), ForceMode2D.Impulse);

                // Ignora a colisão entre a cápsula e o jogador
                Physics2D.IgnoreLayerCollision(shell.layer, playerLayer, true);
            }
            else
            {
                Debug.LogWarning("A cápsula instanciada não possui Rigidbody2D ou Collider2D, a colisão com o jogador não poderá ser ignorada.");
            }

            // Destrói a cápsula após um certo tempo (opcional)
            if (lifeTime > 0)
            {
                Destroy(shell, lifeTime);
            }
        }
        else
        {
            Debug.LogWarning("Shell Prefab ou Shell Ejection Point não foram configurados no script EjetarCapsula em " + gameObject.name);
        }
    }
}