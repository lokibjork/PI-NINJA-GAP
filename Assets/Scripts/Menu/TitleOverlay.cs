using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleOverlay : MonoBehaviour
{
    public GameObject overlayPanel;
    public CanvasGroup canvasGroup;
    public GameObject mainMenuPanel; // Painel do Menu Principal
    public TMP_Text pressAnyKeyText;
    public AudioSource fadeOutSound;
    public float fadeDuration = 1f;
    [SerializeField] private GameObject FirstOption;

    private bool isFading = false;

    private void Start()
    {
        canvasGroup.alpha = 1; // Começa visível
        mainMenuPanel.SetActive(false); // Menu Principal começa desativado
        if (fadeOutSound != null)
            fadeOutSound.playOnAwake = false;
    }

    private void Update()
    {
        if (!isFading && IsAnyKeyButMousePressed())
        {
            StartCoroutine(FadeOutAndActivateMenu());
        }
    }

    private bool IsAnyKeyButMousePressed()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                return false;
            }
            return true;
        }
        return false;
    }

    private IEnumerator FadeOutAndActivateMenu()
    {
        isFading = true;

        if (fadeOutSound != null)
        {
            fadeOutSound.Play();
        }

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // Ajusta a opacidade do Canvas
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            // Desaparecimento gradual do texto
            pressAnyKeyText.color = new Color(
                pressAnyKeyText.color.r,
                pressAnyKeyText.color.g,
                pressAnyKeyText.color.b,
                Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration)
            );

            yield return null;
        }

        overlayPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(FirstOption);// Ativa o menu principal
    }
}

