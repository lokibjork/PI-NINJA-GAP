using UnityEngine;

public class RicoEnemy : EnemyBase
{
    public float attackCooldown = 0.5f;
    public float detectionRange = 8f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float gunDistance = 0.5f;
    public Transform weaponObject; // Objeto que será rotacionado

    private Transform player;
    private float cooldownTimer = 0f;

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        WeaponRot();

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            RotateWeapon();
            if (cooldownTimer <= 0)
            {
                AttackPlayer();
            }
        }

        cooldownTimer -= Time.deltaTime;
        
        
    }

    private void WeaponRot()
    {
        Vector3 playerPos = player.position;
        Vector3 direction = playerPos - transform.position;
        
        weaponObject.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        weaponObject.position = transform.position + Quaternion.Euler(0, 0, angle) * new Vector3(gunDistance, 0, 0);
    }

    void RotateWeapon()
    {
        Vector2 direction = (player.position - weaponObject.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        weaponObject.rotation = Quaternion.Euler(0f, 0f, angle);

        // Chama Flip se necessário
        Flip(direction.x);
    }

    void AttackPlayer()
    {
        cooldownTimer = attackCooldown;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = (player.position - firePoint.position).normalized;
        projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * 5f; // Velocidade do projétil
    }

    new void Flip(float directionX)
    {
        if (directionX > 0 && transform.localScale.x < 0 || directionX < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}