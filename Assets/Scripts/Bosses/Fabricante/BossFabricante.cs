using System.Collections;
using UnityEngine;

public class BossFabricante : EnemyBase
{
    [Header("Boss Settings")]
    public Animator animator;
    public Transform shockwaveSpawnPoint;
    public GameObject shockwavePrefab;
    public GameObject pickaxePrefab;

    [Header("Attack Settings")]
    public float jumpCooldown = 3f;
    public float pickaxeCooldown = 5f;
    public float phaseTwoMultiplier = 1.5f;

    private bool isPhaseTwo = false;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(BossBehavior());
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
        Instantiate(shockwavePrefab, shockwaveSpawnPoint.position, Quaternion.identity);
    }

    private IEnumerator ThrowPickaxe()
    {
        animator.SetTrigger("AtaquePicareta");
        yield return new WaitForSeconds(0.5f);
        Instantiate(pickaxePrefab, transform.position, Quaternion.identity);
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
}
