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
        screenShaker = FindObjectOfType<ScreenShaker>();
    }

    void Update()
    {
        if (isBoomerang)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / journeyTime;

            if (!returning)
            {
                transform.position = Vector3.Lerp(startPosition, startPosition + startDirection * returnDistance, t);

                if (t >= 1f)
                {
                    returning = true;
                    elapsedTime = 0f;
                }
            }
            else
            {
                transform.position = Vector3.Lerp(startPosition + startDirection * returnDistance, startPosition, t);

                if (t >= 1f)
                {
                    Destroy(gameObject);
                }
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
