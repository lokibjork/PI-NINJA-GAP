using System.Collections;
using UnityEngine;

public class BossMinerador : EnemyBase
{
    [Header("Boss Settings")]
    public Animator animator;
    public Transform shockwaveSpawnPoint;
    public GameObject shockwavePrefab;
    public GameObject pickaxePrefab;
    public GameObject player;
    public float offset;

    [Header("Attack Settings")]
    public float jumpCooldown = 5f;
    public float pickaxeCooldown = 8f;
    public float phaseTwoMultiplier = 1.5f;

    [Header("Surround Settings")]
    public float surroundRadius = 3f; // Distância fixa ao redor do jogador
    public float surroundSpeed = 50f; // Velocidade de rotação

    private bool isPhaseTwo = false;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(BossBehavior());
    }

    private void Update()
    {
        SurroundPlayer();
    }

    private IEnumerator BossBehavior()
    {
        while (true)
        {
            if (!isPhaseTwo)
            {
                yield return StartCoroutine(PerformJumpAttack());
                yield return new WaitForSeconds(jumpCooldown);

                yield return StartCoroutine(ThrowPickaxe());
                yield return new WaitForSeconds(pickaxeCooldown);
            }
            else // Segunda fase mais intensa
            {
                yield return StartCoroutine(PerformJumpAttack());
                yield return new WaitForSeconds(jumpCooldown / phaseTwoMultiplier);

                yield return StartCoroutine(ThrowPickaxe());
                yield return new WaitForSeconds(pickaxeCooldown / phaseTwoMultiplier);
            }
        }
    }

    private IEnumerator PerformJumpAttack()
    {
        animator.SetTrigger("Pular");
        yield return new WaitForSeconds(1f);
        Debug.Log("Pulando");
    }

    private IEnumerator ThrowPickaxe()
    {
        animator.SetTrigger("AtaquePicareta");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Jogando a Picareta");
    }

    public new void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        base.TakeDamage(damage, knockbackDirection);

        if (currentHealth <= maxHealth / 2 && !isPhaseTwo)
        {
            isPhaseTwo = true;
            animator.SetTrigger("PhaseTwo");
        }
    }

    private void SurroundPlayer()
    {
        float angle = Time.time * surroundSpeed;
        float x = Mathf.Cos(angle) * surroundRadius;
        float z = Mathf.Sin(angle) * surroundRadius;

        transform.position = player.transform.position + new Vector3(x, 0, z);
    }
}
