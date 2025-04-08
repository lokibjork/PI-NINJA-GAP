using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InteractableItem : MonoBehaviour
{
    [Header("Configurações")]
    public float interactionRange = 2f; // Distância para poder interagir
    public GameObject interactionIcon; // Ícone de interação (UI)
    public float fadeSpeed = 2f; // Velocidade do fade

    [Header("Eventos")]
    public UnityEvent onInteract; // Ações que acontecem ao interagir

    private Transform player;
    private bool isPlayerNear;
    private CanvasGroup iconCanvasGroup;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        iconCanvasGroup = interactionIcon.GetComponent<CanvasGroup>();
        iconCanvasGroup.alpha = 0; // Começa invisível
        interactionIcon.SetActive(false);
    }

    void Update()
    {
        // Verifica distância do jogador
        float distance = Vector3.Distance(transform.position, player.position);
        bool shouldBeNear = distance <= interactionRange;

        // Se o estado mudou (entrou/saiu da área)
        if (shouldBeNear != isPlayerNear)
        {
            isPlayerNear = shouldBeNear;

            if (isPlayerNear)
            {
                // Mostra ícone com fade in
                interactionIcon.SetActive(true);
                StartCoroutine(FadeIcon(1f)); // Fade para visível
            }
            else
            {
                // Esconde ícone com fade out
                StartCoroutine(FadeIcon(0f, hideAfter: true));
            }
        }

        // Verifica interação (seta para baixo)
        if (isPlayerNear && Input.GetButtonDown("Vertical"))
        {
            onInteract.Invoke(); // Dispara o evento de interação
        }
    }

    IEnumerator FadeIcon(float targetAlpha, bool hideAfter = false)
    {
        while (!Mathf.Approximately(iconCanvasGroup.alpha, targetAlpha))
        {
            iconCanvasGroup.alpha = Mathf.MoveTowards(
                iconCanvasGroup.alpha,
                targetAlpha,
                fadeSpeed * Time.deltaTime
            );
            yield return null;
        }

        if (hideAfter && iconCanvasGroup.alpha == 0f)
        {
            interactionIcon.SetActive(false);
        }
    }

    // Desenha um gizmo para visualizar o range no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}