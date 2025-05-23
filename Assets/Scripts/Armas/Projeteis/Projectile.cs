using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public int damage;
    public float lifetime;
    public bool isBouncing;
    public bool isBoomerang;
    public bool isRocket;
    private int bounceCount = 0;
    public int maxBounces = 6;

    private Vector3 startDirection;
    public bool returning = false;
    private Vector3 startPosition;
    private float journeyTime = 0.5f;
    private float elapsedTime = 0f;
    public float returnDistance = 6f;
    private ScreenShaker screenShaker;

    void Start()
    {
        Destroy(gameObject, lifetime);
        startDirection = transform.rotation * Vector3.right;
        startPosition = transform.position;
#pragma warning disable CS0618 // Type or member is obsolete
        screenShaker = FindObjectOfType<ScreenShaker>();
#pragma warning restore CS0618 // Type or member is obsolete
    }

    void Update()
    {
        if (isBoomerang)
        {
            if (!returning)
            {
                // Move-se para frente at� atingir returnDistance
                transform.position += startDirection * (speed * Time.deltaTime);

                // Verifica se atingiu a dist�ncia m�xima
                if (Vector3.Distance(startPosition, transform.position) >= returnDistance)
                {
                    returning = true;
                    startDirection = -startDirection; // Inverte a dire��o!
                }
            }
            else
            {
                // Continua movendo na dire��o invertida
                transform.position += startDirection * (speed * Time.deltaTime);
            }
        }
        else if (isBouncing)
        {
            transform.position += startDirection * (speed * Time.deltaTime);
        }
        else if (isRocket)
        {
            transform.Translate(Vector2.right * (speed * Time.deltaTime));
        }
        else
        {
            transform.Translate(Vector2.right * (speed * Time.deltaTime));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            screenShaker?.Shake(Vector2.down * screenShaker.shakeForce);
            
            if (isRocket)
            {
                Explode();
            }
            else if (!isBouncing && !isBoomerang)
            {
                Destroy(gameObject);
            }
        }

        if (isBouncing)
        {
            startDirection = Vector2.Reflect(startDirection, collision.contacts[0].normal);
            bounceCount++;

            if (bounceCount >= maxBounces)
            {
                Destroy(gameObject);
            }
        }
        
        if (isRocket)
        {
            Explode();
        }

        if (!isBouncing && !isBoomerang && !isRocket)
        {
            Destroy(gameObject);
        }
    }
    
    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                
            }
        }
        Destroy(gameObject);
    }
}
