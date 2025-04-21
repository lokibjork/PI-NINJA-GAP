using UnityEngine;

public class ProjetilKnockback : MonoBehaviour
{
    [Header("Configurações do Knockback da Bala")]
    public float knockbackForca = 5f;
    public string inimigoTag = "Enemy";
    public GameObject efeitoSanguePrefab;
    public float timer = 1f; // Valor padrão para o timer, ajuste no Inspector

    private Rigidbody2D rb;
    private bool impactProcessed = false; // Nova variável de controle

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("O projétil não possui um Rigidbody2D. Adicione um para o knockback funcionar.");
            enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(inimigoTag) && !impactProcessed)
        {
            ProcessarImpacto(other, other.ClosestPoint(transform.position));
            impactProcessed = true; // Marca o impacto como processado
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(inimigoTag) && !impactProcessed)
        {
            ProcessarImpacto(collision.collider, collision.GetContact(0).point);
            impactProcessed = true; // Marca o impacto como processado
        }
    }

    void ProcessarImpacto(Collider2D inimigoCollider, Vector2 pontoDeContato)
    {
        Rigidbody2D inimigoRb = inimigoCollider.GetComponent<Rigidbody2D>();
        if (inimigoRb != null)
        {
            Vector2 knockbackDirection = -rb.linearVelocity.normalized;
            inimigoRb.AddForce(knockbackDirection * knockbackForca, ForceMode2D.Impulse);

            if (efeitoSanguePrefab != null)
            {
                // 1. Calcula a direção do ponto de contato para o centro do inimigo
                Vector2 direcaoContatoParaCentro = inimigoCollider.bounds.center - (Vector3)pontoDeContato;
                Vector2 direcaoOpostaContato = -direcaoContatoParaCentro.normalized;

                // 2. Calcula a posição de spawn do efeito (ligeiramente afastado do ponto de contato)
                Vector2 posicaoSpawnEfeito = pontoDeContato + direcaoOpostaContato * 0.15f; // Ajuste o valor conforme necessário

                // 3. Instancia o efeito de sangue, alinhando sua rotação com a direção oposta
                GameObject efeitoSangue = Instantiate(efeitoSanguePrefab, posicaoSpawnEfeito, Quaternion.LookRotation(Vector3.forward, direcaoOpostaContato));

                Destroy(efeitoSangue, timer);
            }

            Destroy(gameObject);
        }
    }
}