using UnityEngine;

public class PicaretaBoomerang : MonoBehaviour
{
    [Header("Configura��es")]
    public float speed;
    public float maxDistance = 10f;
    public int damage = 2;
    public Vector2 direction;
    public bool isPhase2 = false;

    private Rigidbody2D rb;
    private Vector2 launchPoint;
    private bool isReturning = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        launchPoint = transform.position;
        rb.linearVelocity = direction * speed;
    }

    void Update()
    {
        // Rota��o visual  
        transform.Rotate(0f, 0f, 360f * Time.deltaTime);

        // L�gica de retorno  
        if (!isReturning && Vector2.Distance(launchPoint, transform.position) >= maxDistance)
        {
            isReturning = true;
            rb.linearVelocity = -rb.linearVelocity;
        }

        // Destr�i se voltar ao ponto inicial (ou ap�s tempo)  
        if (isReturning && Vector2.Distance(launchPoint, transform.position) < 0.5f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<DamageHandler>().TakeDamage(damage, transform.position, 1f);
            // Knockback opcional  
            Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
            other.GetComponent<Rigidbody2D>().AddForce(knockbackDir * 3f, ForceMode2D.Impulse);
        }
    }
}