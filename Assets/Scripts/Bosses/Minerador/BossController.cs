using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class BossController : MonoBehaviour
{
    public enum BossState
    {
        MissileAttack,
        LaserAttack,
        Idle // Opcional
    }

    public BossState currentState = BossState.MissileAttack;
    public float stateChangeInterval = 5f; // Tempo em segundos para trocar de ataque
    private float nextStateChangeTime;
    [SerializeField] private bool isChangingState = false; // Flag para controlar a troca de estado

    [Header("Ataque de Mísseis")]
    public Transform leftHandSpawnPoint;
    public Transform rightHandSpawnPoint;
    public GameObject homingMissilePrefab;
    public float missileSpawnInterval = 2f;
    private float nextMissileSpawnTime;

    [Header("Ataque de Lasers Animados")]
    public GameObject animatedLaserPrefab; // Prefab do laser animado com trigger
    public float laserSpawnInterval = 0.8f; // Intervalo entre o spawn de cada laser
    public int laserBurstCount = 2; // Número de lasers a serem disparados por "rajada"
    public float laserAttackDuration = 4f; // Duração total do estado de laser
    private float nextLaserSpawnTime;
    private int lasersSpawnedThisBurst = 0;
    private float laserAttackStartTime;
    private bool isLaserAttacking = false;

    [Header("Pontos de Spawn de Laser")]
    public Transform laserSpawnPoint1;
    public Transform laserSpawnPoint2;
    private List<Transform> laserSpawnPoints = new List<Transform>();
    private int currentLaserSpawnIndex = 0;

    [Header("Rotação do Laser Invertido")]
    public Vector3 invertedLaserRotation = new Vector3(0f, 180f, 0f); // Rotação para o segundo laser

    [Header("Invulnerabilidade do Laser")]
    [SerializeField] public float invulnerabilityDuration = 4f; // Deve corresponder à duração do laser
    [SerializeField] private float invulnerabilityTimer = 0f;
    [SerializeField] private bool isVulnerable = true;

    [Header("Iluminação")]
    public Light2D arenaLight; // Referência à luz global da arena
    public float dimLightIntensity = 0.1f;
    public float normalLightIntensity = 1f; // Inicialize aqui ou no Inspector

    void Start()
    {
        nextMissileSpawnTime = Time.time + missileSpawnInterval;
        nextStateChangeTime = Time.time + stateChangeInterval;
        UpdateBossVisualState();

        if (arenaLight != null) normalLightIntensity = arenaLight.intensity;

        // Inicializa a lista de pontos de spawn de laser
        if (laserSpawnPoint1 != null) laserSpawnPoints.Add(laserSpawnPoint1);
        if (laserSpawnPoint2 != null) laserSpawnPoints.Add(laserSpawnPoint2);
    }

    void Update()
    {
        // Controle da troca de estados
        if (Time.time >= nextStateChangeTime && !isChangingState)
        {
            isChangingState = true;
            ChangeState();
            nextStateChangeTime = Time.time + stateChangeInterval;
        }

        // Lógica de ataque baseada no estado atual
        switch (currentState)
        {
            case BossState.MissileAttack:
                HandleMissileAttack();
                // Restaura a luz se voltamos para o modo de mísseis
                if (arenaLight != null && arenaLight.intensity != normalLightIntensity)
                {
                    arenaLight.intensity = normalLightIntensity;
                }
                isLaserAttacking = false; // Garante que a flag do laser seja resetada
                lasersSpawnedThisBurst = 0;
                currentLaserSpawnIndex = 0; // Reseta o índice de spawn do laser
                break;
            case BossState.LaserAttack:
                HandleLaserAttack();
                // Diminui a luz ao iniciar o ataque laser
                if (!isLaserAttacking && arenaLight != null)
                {
                    arenaLight.intensity = dimLightIntensity;
                    isLaserAttacking = true;
                    laserAttackStartTime = Time.time;
                    nextLaserSpawnTime = Time.time + laserSpawnInterval;
                    lasersSpawnedThisBurst = 0;
                    currentLaserSpawnIndex = 0; // Reseta o índice de spawn do laser
                    isVulnerable = false;
                    invulnerabilityTimer = invulnerabilityDuration;
                }
                break;
            case BossState.Idle:
                // Comportamento de descanso
                break;
        }

        // Reset da flag de troca de estado após um frame
        isChangingState = false;

        // Controle da invulnerabilidade durante o laser
        if (!isVulnerable && currentState == BossState.LaserAttack && Time.time >= laserAttackStartTime + invulnerabilityDuration)
        {
            isVulnerable = true;
            if (arenaLight != null) arenaLight.intensity = normalLightIntensity;
            isLaserAttacking = false; // Finaliza o ataque de laser após a duração
            lasersSpawnedThisBurst = 0;
            currentLaserSpawnIndex = 0; // Reseta o índice de spawn do laser
        }
    }

    void ChangeState()
    {
        if (currentState == BossState.Idle)
        {
            currentState = BossState.LaserAttack;
        }
        else if (currentState == BossState.MissileAttack)
        {
            currentState = BossState.LaserAttack;
        }
        else if (currentState == BossState.LaserAttack)
        {
            currentState = BossState.MissileAttack;
        }
        UpdateBossVisualState();
    }

    void UpdateBossVisualState()
    {
        if (currentState == BossState.LaserAttack)
        {
            Debug.Log("Chefe entrando no modo de Laser!");
            // Desativar visual das mãos de míssil (opcional)
            // Ativar visual dos emissores de laser (opcional)
        }
        else if (currentState == BossState.MissileAttack)
        {
            Debug.Log("Chefe entrando no modo de Míssil!");
            // Ativar visual das mãos de míssil (opcional)
            // Desativar visual dos emissores de laser (opcional)
        }
    }

    void HandleMissileAttack()
    {
        if (currentState == BossState.MissileAttack && Time.time >= nextMissileSpawnTime)
        {
            SpawnMissile(leftHandSpawnPoint);
            SpawnMissile(rightHandSpawnPoint);
            nextMissileSpawnTime = Time.time + missileSpawnInterval;
        }
    }

    void HandleLaserAttack()
    {
        if (currentState == BossState.LaserAttack && Time.time >= nextLaserSpawnTime && lasersSpawnedThisBurst < laserBurstCount && Time.time < laserAttackStartTime + laserAttackDuration && laserSpawnPoints.Count > 0)
        {
            Transform spawnPoint = laserSpawnPoints[currentLaserSpawnIndex % laserSpawnPoints.Count];
            Quaternion laserRotation = spawnPoint.rotation;

            // Se for o segundo ponto de spawn (índice 1), aplica a rotação invertida
            if (currentLaserSpawnIndex % laserSpawnPoints.Count == 1)
            {
                laserRotation *= Quaternion.Euler(invertedLaserRotation);
            }

            SpawnLaser(spawnPoint, laserRotation);
            currentLaserSpawnIndex++;
            nextLaserSpawnTime = Time.time + laserSpawnInterval;
            lasersSpawnedThisBurst++;
        }
        // Após a duração total do ataque de laser, o estado será trocado no Update
    }

    void SpawnMissile(Transform spawnPoint)
    {
        if (homingMissilePrefab != null && spawnPoint != null)
        {
            Instantiate(homingMissilePrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    void SpawnLaser(Transform spawnPoint, Quaternion rotation)
    {
        if (animatedLaserPrefab != null && spawnPoint != null)
        {
            Instantiate(animatedLaserPrefab, spawnPoint.position, rotation);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isVulnerable)
        {
            Debug.Log("Chefe recebeu " + damage + " de dano!");
        }
        else
        {
            Debug.Log("Chefe invulnerável!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bala"))
        {

        }
    }
}