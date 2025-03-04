using UnityEngine;

public class Robo : EnemyBase
{
    public float moveSpeed = 3f;
    public float jumpForce = 7f;
    public int damage = 5;
    public Transform groundCheck;
    public Transform wallCheck;
    public string groundTag = "Ground";
    public string wallTag = "Wall"; // Nova tag para paredes
    private Transform target;
    private bool isGrounded;

    protected override void Start()
    {
        base.Start();
        maxHealth = 10;
        currentHealth = maxHealth;
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        CheckForGroundAhead();
        CheckForWall();
        Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Follow();
        }

        if (collision.CompareTag(groundTag))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(groundTag))
        {
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerData player = collision.gameObject.GetComponent<PlayerData>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
        else if (collision.gameObject.CompareTag(groundTag) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void Move()
    {
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
    }

    private void Follow()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    private void CheckForGroundAhead()
    {
        Vector2 rayOrigin = groundCheck.position;
        Vector2 rayDirection = Vector2.down;
        float rayDistance = 1f;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);

        if (hit.collider == null || !hit.collider.CompareTag(groundTag))
        {
            Flip();
        }
    }

    private void CheckForWall()
    {
        Vector2 rayOrigin = wallCheck.position;
        Vector2 rayDirection = transform.right;
        float rayDistance = 0.5f;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.blue);

        if (hit.collider != null && hit.collider.CompareTag(wallTag))
        {
            Flip();
        }
    }

    private void Flip()
    {
        moveSpeed = -moveSpeed;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
