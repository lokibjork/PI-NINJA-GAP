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

    [Header("Ataque de M�sseis")]
    public Transform leftHandSpawnPoint;
    public Transform rightHandSpawnPoint;
    public GameObject homingMissilePrefab;
    public float missileSpawnInterval = 2f;
    private float nextMissileSpawnTime;

    [Header("Ataque de Lasers Animados")]
    public GameObject animatedLaserPrefab; // Prefab do laser animado com trigger
    public float laserSpawnInterval = 0.8f; // Intervalo entre o spawn de cada laser
    public int laserBurstCount = 2; // N�mero de lasers a serem disparados por "rajada"
    public float laserAttackDuration = 4f; // Dura��o total do estado de laser
    private float nextLaserSpawnTime;
    private int lasersSpawnedThisBurst = 0;
    private float laserAttackStartTime;
    private bool isLaserAttacking = false;
    public float laserAttackShakeIntensity = 0.08f; // Intensidade do shake durante o laser
    public float laserAttackShakeDuration = 8f; // Dura��o total do per�odo de shake em segundos
    private float laserAttackShakeEndTime; // Tempo em que o shake deve parar

    [Header("Pontos de Spawn de Laser")]
    public Transform laserSpawnPoint1;
    public Transform laserSpawnPoint2;
    private List<Transform> laserSpawnPoints = new List<Transform>();
    private int currentLaserSpawnIndex = 0;

    [Header("Rota��o do Laser Invertido")]
    public Vector3 invertedLaserRotation = new Vector3(0f, 180f, 0f); // Rota��o para o segundo laser

    [Header("Invulnerabilidade do Laser")]
    [SerializeField] public float invulnerabilityDuration = 4f; // Deve corresponder � dura��o do laser
    [SerializeField] private float invulnerabilityTimer = 0f;
    [SerializeField] public bool isVulnerable = true;

    [Header("Ilumina��o")]
    public Light2D arenaLight; // Refer�ncia � luz global da arena
    public float dimLightIntensity = 0.1f;
    public float normalLightIntensity = 1f; // Inicialize aqui ou no Inspector
    public float lightBlinkIntensity = 0.5f; // Intensidade da luz durante a piscada
    public float lightBlinkSpeed = 0.1f; // Dura��o de cada piscada em segundos
    public int lightBlinkCount = 3; // N�mero de piscadas antes de apagar

    private ScreenShaker screenShaker; // Refer�ncia ao ScreenShaker

    void Start()
    {
        nextMissileSpawnTime = Time.time + missileSpawnInterval;
        nextStateChangeTime = Time.time + stateChangeInterval;
        UpdateBossVisualState();

        if (arenaLight != null) normalLightIntensity = arenaLight.intensity;

        // Inicializa a lista de pontos de spawn de laser
        if (laserSpawnPoint1 != null) laserSpawnPoints.Add(laserSpawnPoint1);
        if (laserSpawnPoint2 != null) laserSpawnPoints.Add(laserSpawnPoint2);

        // Busca o ScreenShaker na cena
        screenShaker = FindObjectOfType<ScreenShaker>();
        if (screenShaker == null)
        {
            Debug.LogWarning("ScreenShaker n�o encontrado na cena para o Boss!");
        }
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

        // L�gica de ataque baseada no estado atual
        switch (currentState)
        {
            case BossState.MissileAttack:
                HandleMissileAttack();
                // Restaura a luz se voltamos para o modo de m�sseis
                if (arenaLight != null && arenaLight.intensity != normalLightIntensity)
                {
                    arenaLight.intensity = normalLightIntensity;
                }
                isLaserAttacking = false; // Garante que a flag do laser seja resetada
                lasersSpawnedThisBurst = 0;
                currentLaserSpawnIndex = 0; // Reseta o �ndice de spawn do laser
                StopCoroutine(BlinkLightsBeforeLaser()); // Garante que a corrotina de piscada seja interrompida se o estado mudar
                break;
            case BossState.LaserAttack:
                HandleLaserAttack();
                // Inicia a sequ�ncia de piscada e diminui��o da luz ao iniciar o ataque laser
                if (!isLaserAttacking && arenaLight != null)
                {
                    StartCoroutine(BlinkLightsBeforeLaser());
                    isLaserAttacking = true;
                    laserAttackStartTime = Time.time;
                    nextLaserSpawnTime = Time.time + laserSpawnInterval;
                    lasersSpawnedThisBurst = 0;
                    currentLaserSpawnIndex = 0; // Reseta o �ndice de spawn do laser
                    isVulnerable = false;
                    invulnerabilityTimer = invulnerabilityDuration;
                    laserAttackShakeEndTime = Time.time + laserAttackShakeDuration; // Define o tempo de fim do shake
                }
                break;
            case BossState.Idle:
                // Comportamento de descanso
                StopCoroutine(BlinkLightsBeforeLaser()); // Garante que a corrotina de piscada seja interrompida se o estado mudar
                break;
        }

        // Reset da flag de troca de estado ap�s um frame
        isChangingState = false;

        // Controle da invulnerabilidade durante o laser
        if (!isVulnerable && currentState == BossState.LaserAttack && Time.time >= laserAttackStartTime + invulnerabilityDuration)
        {
            isVulnerable = true;
            if (arenaLight != null) arenaLight.intensity = normalLightIntensity;
            isLaserAttacking = false; // Finaliza o ataque de laser ap�s a dura��o
            lasersSpawnedThisBurst = 0;
            currentLaserSpawnIndex = 0; // Reseta o �ndice de spawn do laser
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
            // Desativar visual das m�os de m�ssil (opcional)
            // Ativar visual dos emissores de laser (opcional)
        }
        else if (currentState == BossState.MissileAttack)
        {
            Debug.Log("Chefe entrando no modo de M�ssil!");
            // Ativar visual das m�os de m�ssil (opcional)
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
        if (currentState == BossState.LaserAttack && Time.time >= nextLaserSpawnTime && lasersSpawnedThisBurst < laserBurstCount && Time.time < laserAttackStartTime + laserAttackDuration && laserSpawnPoints.Count > 0 && Time.time < laserAttackShakeEndTime)
        {
            Transform spawnPoint = laserSpawnPoints[currentLaserSpawnIndex % laserSpawnPoints.Count];
            Quaternion laserRotation = spawnPoint.rotation;

            if (currentLaserSpawnIndex % laserSpawnPoints.Count == 1)
            {
                laserRotation *= Quaternion.Euler(invertedLaserRotation);
            }

            SpawnLaser(spawnPoint, laserRotation);

            if (screenShaker != null)
            {
                screenShaker.Shake(Vector2.up * laserAttackShakeIntensity);
            }

            currentLaserSpawnIndex++;
            nextLaserSpawnTime = Time.time + laserSpawnInterval;
            lasersSpawnedThisBurst++;
        }
        // Ap�s a dura��o total do ataque de laser, o estado ser� trocado no Update
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
            GameObject laser = Instantiate(animatedLaserPrefab, spawnPoint.position, rotation);
            Destroy(laser, 2f);
        }
    }

    public void TakeDamage(int damage)
    {
        // A l�gica de dano agora est� no BossHealth script
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bala"))
        {
            // A l�gica de dano agora est� no BossHealth script
        }
    }

    IEnumerator BlinkLightsBeforeLaser()
    {
        if (arenaLight == null) yield break;

        float originalIntensity = arenaLight.intensity;

        for (int i = 0; i < lightBlinkCount; i++)
        {
            arenaLight.intensity = lightBlinkIntensity;
            yield return new WaitForSeconds(lightBlinkSpeed);
            arenaLight.intensity = originalIntensity;
            yield return new WaitForSeconds(lightBlinkSpeed);
        }

        arenaLight.intensity = dimLightIntensity;
    }
}