using System.Collections;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    [Header("Configurações da Plataforma")]
    public float breakDelay = 1.5f; // Tempo que o jogador precisa ficar parado na plataforma antes dela quebrar
    public float respawnTime = 3f; // Tempo para a plataforma reaparecer

    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private bool isBreaking = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isBreaking)
        {
            StartCoroutine(BreakPlatform());
        }
    }

    private IEnumerator BreakPlatform()
    {
        isBreaking = true;
        yield return new WaitForSeconds(breakDelay);

        // Some efeito de partículas ou som aqui se quiser
        spriteRenderer.enabled = false;
        platformCollider.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        // Plataforma reaparece
        spriteRenderer.enabled = true;
        platformCollider.enabled = true;
        isBreaking = false;
    }
}