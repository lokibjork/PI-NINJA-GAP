using System.Collections;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    [Header("Configurações da Plataforma")]
    public float breakDelay = 1f; // Tempo que o jogador precisa ficar parado na plataforma antes de desaparecer
    public float respawnTime = 1f; // Tempo para a plataforma reaparecer

    private Collider2D platformCollider;
    private bool isBreaking = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Start()
    {
        platformCollider = GetComponent<Collider2D>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isBreaking)
        {
            StartCoroutine(BreakAndRespawn());
        }
    }

    private IEnumerator BreakAndRespawn()
    {
        isBreaking = true;
        // Desativa o collider imediatamente para evitar colisões múltiplas
        if (platformCollider != null)
        {
            platformCollider.enabled = false;
        }

        // Oculta a plataforma (opcional, você pode adicionar efeitos visuais aqui)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }

        yield return new WaitForSeconds(breakDelay);

        // Destrói a plataforma antiga
        Destroy(gameObject);

        // Espera o tempo de respawn
        yield return new WaitForSeconds(respawnTime);

        // Instancia uma nova plataforma no mesmo local e rotação
        GameObject newPlatform = Instantiate(gameObject, originalPosition, originalRotation);

        // Garante que a nova plataforma não inicie o processo de quebra imediatamente
        BreakablePlatform newPlatformScript = newPlatform.GetComponent<BreakablePlatform>();
        if (newPlatformScript != null)
        {
            newPlatformScript.isBreaking = false;
        }
    }
}