using UnityEngine;

public class RicoEnemy : EnemyBase
{
    public float attackCooldown = 0.5f;
    public float detectionRange = 8f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float gunDistance = 0.5f;
    public Transform weaponObject;

    public bool isfacingRight;
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
        FlipFunction();  

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (cooldownTimer <= 0)
            {
                AttackPlayer();
            }
        }

        cooldownTimer -= Time.deltaTime;
    }

    private void FlipFunction()
    {
        if (player.position.x < weaponObject.position.x && isfacingRight)
        {
            FlipWeapon(weaponObject);
        }else if (player.position.x > weaponObject.position.x && !isfacingRight)
        {
            FlipWeapon(weaponObject);
        }
    }

    private void WeaponRot()
    {
        Vector3 playerPos = player.position;
        Vector3 direction = playerPos - transform.position;
        
        weaponObject.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        weaponObject.position = transform.position + Quaternion.Euler(0, 0, angle) * new Vector3(gunDistance, 0, 0);
    }

    void FlipWeapon(Transform weapon)
    {
        isfacingRight = !isfacingRight;
        weapon.localScale = new Vector3(weapon.localScale.x, weapon.localScale.y * -1, weapon.localScale.z);
    }

    void AttackPlayer()
    {
        cooldownTimer = attackCooldown;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = (player.position - firePoint.position).normalized;
        projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * 15f;
        Destroy(projectilePrefab, 1.5f);
    }
}
