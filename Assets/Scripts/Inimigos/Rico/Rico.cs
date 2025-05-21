using UnityEngine;



public class RicoEnemy : EnemyBase

{

    public float attackCooldown = 0.5f;

    public float detectionRange = 8f;

    public GameObject projectilePrefab;

    public Transform firePoint;

    public float gunDistance = 0.5f;

    public Transform weaponObject;



    public bool isFacingRight { get; private set; } = true; // Propriedade com getter privado

    private Transform player;

    private float cooldownTimer = 0f;

    private SpriteRenderer enemySpriteRenderer;



    protected override void Start()

    {

        base.Start();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        enemySpriteRenderer = GetComponent<SpriteRenderer>();

        if (enemySpriteRenderer == null)

        {

            Debug.LogError("SpriteRenderer n�o encontrado no inimigo Rico!");

        }



        // Detecta a dire��o inicial baseada no scale

        isFacingRight = transform.localScale.x > 0;

    }



    void Update()

    {

        WeaponRot();

        UpdateFacingDirection();



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



    private void UpdateFacingDirection()

    {

        if (player.position.x < transform.position.x && isFacingRight)

        {

            Flip();

        }

        else if (player.position.x > transform.position.x && !isFacingRight)

        {

            Flip();

        }

    }



    private void Flip()

    {

        isFacingRight = !isFacingRight;

        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        if (weaponObject != null)

        {

            weaponObject.localScale = new Vector3(weaponObject.localScale.x, -weaponObject.localScale.y, weaponObject.localScale.z); // Flip vertical da arma (ajuste conforme necess�rio)

        }

    }



    private void WeaponRot()

    {

        if (player == null || weaponObject == null) return; // Seguran�a caso o jogador ou a arma n�o sejam encontrados



        Vector3 playerPos = player.position;

        Vector3 direction = playerPos - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;



        weaponObject.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        weaponObject.position = transform.position + Quaternion.Euler(0, 0, angle) * new Vector3(gunDistance, 0, 0);

    }



    void AttackPlayer()

    {

        cooldownTimer = attackCooldown;

        if (projectilePrefab != null && firePoint != null && player != null)

        {

            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            Vector2 direction = (player.position - firePoint.position).normalized;

            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

            if (projectileRb != null)

            {

                projectileRb.linearVelocity = direction * 15f; // Use velocity em vez de linearVelocity para consist�ncia

                Destroy(projectile, 1.5f);

            }

            else

            {

                Debug.LogError("Rigidbody2D n�o encontrado no proj�til!");

                Destroy(projectile); // Garante que o proj�til seja destru�do mesmo sem Rigidbody

            }

        }

        else

        {

            Debug.LogError("Prefab de proj�til, firePoint ou jogador n�o referenciado!");

        }

    }

}