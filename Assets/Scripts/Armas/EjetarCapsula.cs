using UnityEngine;

public class EjetarCapsula : MonoBehaviour
{
    [Header("Configura��es da C�psula")]
    public GameObject shellPrefab; // Prefab da c�psula da bala
    public Transform shellEjectionPoint; // Ponto de onde a c�psula ser� ejetada
    public float ejectionForce = 10f; // For�a de eje��o da c�psula
    public float torqueForce = 2f; // For�a de rota��o da c�psula
    public float lifeTime = 2f; // Tempo em segundos antes da c�psula ser destru�da (opcional)
    public GameObject smokePrefab;
    public Transform smokePoint;

    [Header("Configura��es de Colis�o")]
    public string playerLayerName = "Player"; // Nome da layer do jogador
    private int playerLayer;

    void Start()
    {
        playerLayer = LayerMask.NameToLayer(playerLayerName);
        if (playerLayer == -1)
        {
            Debug.LogError("Layer '" + playerLayerName + "' n�o encontrada. Certifique-se de que a layer do jogador est� configurada corretamente.");
            enabled = false;
        }
    }

    public void Ejetar()
    {
        if (shellPrefab != null && shellEjectionPoint != null)
        {
            // Instancia a c�psula
            GameObject shell = Instantiate(shellPrefab, shellEjectionPoint.position, shellEjectionPoint.rotation);

            // Obt�m o componente Rigidbody2D da c�psula (se houver)
            Rigidbody2D shellRb = shell.GetComponent<Rigidbody2D>();
            Collider2D shellCollider = shell.GetComponent<Collider2D>();
            if (shellRb != null && shellCollider != null)
            {
                // Calcula a dire��o de eje��o baseada na rota��o do ponto de eje��o
                Vector2 ejectionDirection = shellEjectionPoint.up;

                // Aplica uma for�a para ejetar a c�psula
                shellRb.AddForce(ejectionDirection * ejectionForce, ForceMode2D.Impulse);

                // Aplica um torque (rota��o) aleat�rio
                shellRb.AddTorque(Random.Range(-torqueForce, torqueForce), ForceMode2D.Impulse);

                // Ignora a colis�o entre a c�psula e o jogador
                Physics2D.IgnoreLayerCollision(shell.layer, playerLayer, true);
            }
            else
            {
                Debug.LogWarning("A c�psula instanciada n�o possui Rigidbody2D ou Collider2D, a colis�o com o jogador n�o poder� ser ignorada.");
            }

            // Destr�i a c�psula ap�s um certo tempo (opcional)
            if (lifeTime > 0)
            {
                Destroy(shell, lifeTime);
            }
        }
        else
        {
            Debug.LogWarning("Shell Prefab ou Shell Ejection Point n�o foram configurados no script EjetarCapsula em " + gameObject.name);
        }
    }
}