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
    private Transform playerTransform; // Renomeado para playerTransform
    private float cooldownTimer = 0f;
    private SpriteRenderer enemySpriteRenderer;

    protected override void Start()
    {
        base.Start();
        FindPlayer(); // Encontra o jogador no início
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
        if (enemySpriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer não encontrado no inimigo Rico!");
        }

        // Detecta a direção inicial baseada no scale
        isFacingRight = transform.localScale.x > 0;
    }

    void OnEnable()
    {
        DamageHandler.OnPlayerDeath += HandlePlayerDeath;
    }

    void OnDisable()
    {
        DamageHandler.OnPlayerDeath -= HandlePlayerDeath;
    }

    void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("RicoEnemy: Não conseguiu encontrar o GameObject do Jogador com a tag 'Player'.");
            enabled = false; // Desativa se o jogador não for encontrado
        }
    }

    void HandlePlayerDeath()
    {
        playerTransform = null;
        Debug.Log("RicoEnemy: Jogador morreu, parando de rastrear.");
        enabled = false; // Ou outra ação apropriada
    }

    void Update()
    {
        if (playerTransform == null) return; // Não faz nada se o jogador não existir

        WeaponRot();
        UpdateFacingDirection();

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

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
        if (playerTransform != null) // Verifica se a referência ainda é válida
        {
            if (playerTransform.position.x < transform.position.x && isFacingRight)
            {
                Flip();
            }
            else if (playerTransform.position.x > transform.position.x && !isFacingRight)
            {
                Flip();
            }
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        if (weaponObject != null)
        {
            weaponObject.localScale = new Vector3(weaponObject.localScale.x, -weaponObject.localScale.y, weaponObject.localScale.z); // Flip vertical da arma (ajuste conforme necessário)
        }
    }

    private void WeaponRot()
    {
        if (playerTransform == null || weaponObject == null) return; // Segurança caso o jogador ou a arma não sejam encontrados

        Vector3 playerPos = playerTransform.position;
        Vector3 direction = playerPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        weaponObject.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        weaponObject.position = transform.position + Quaternion.Euler(0, 0, angle) * new Vector3(gunDistance, 0, 0);
    }

    void AttackPlayer()
    {
        cooldownTimer = attackCooldown;
        if (projectilePrefab != null && firePoint != null && playerTransform != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Vector2 direction = (playerTransform.position - firePoint.position).normalized;
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            if (projectileRb != null)
            {
                projectileRb.linearVelocity = direction * 15f; // Use velocity em vez de linearVelocity para consistência
                Destroy(projectile, 1.5f);
            }
            else
            {
                Debug.LogError("Rigidbody2D não encontrado no projétil!");
                Destroy(projectile); // Garante que o projétil seja destruído mesmo sem Rigidbody
            }
        }
        else
        {
            string missingReferences = "";
            if (projectilePrefab == null) missingReferences += " projectilePrefab";
            if (firePoint == null) missingReferences += " firePoint";
            if (playerTransform == null) missingReferences += " playerTransform";
            Debug.LogError("RicoEnemy: Referências não atribuídas (" + missingReferences.Trim() + ")!");
        }
    }
}