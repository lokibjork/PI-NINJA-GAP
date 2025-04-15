using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroSequence : MonoBehaviour
{
    public Image imageDisplay;
    public Sprite[] introImages;

    public GameObject promptButton;
    private RectTransform promptRect;
    private CanvasGroup promptCanvasGroup;

    public float fadeDuration = 0.5f;
    public float bounceAmount = 10f;
    public float bounceSpeed = 0.5f;

    private int currentIndex = 0;
    private bool canAdvance = false;
    private int bounceTweenId;

    [SerializeField] private AudioClip[] imageSounds;
    [SerializeField] private AudioClip nextSound;

    public MonoBehaviour[] scriptsToDisable;

    void Start()
    {
        // Descomente se quiser ignorar a cutscene depois de vista
        if (PlayerPrefs.GetInt("IntroSeen", 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }

        promptCanvasGroup = promptButton.GetComponent<CanvasGroup>();
        promptRect = promptButton.GetComponent<RectTransform>();

        promptButton.SetActive(false);
        DisableGameplay();
        ShowImage(0);
    }

    void Update()
    {
        if (canAdvance && Input.GetButtonDown("Fire1"))
        {
            Advance();
        }
    }

    void ShowImage(int index)
    {
        imageDisplay.sprite = introImages[index];

        // Toca som da imagem
        if (imageSounds != null && index < imageSounds.Length && imageSounds[index] != null)
        {
            SoundManagerSO.PlaySoundFXClip(imageSounds[index], transform.position, 1f);
        }

        ShowPrompt();
    }

    void ShowPrompt()
    {
        promptButton.SetActive(true);
        promptCanvasGroup.alpha = 0;
        LeanTween.alphaCanvas(promptCanvasGroup, 1f, fadeDuration);

        bounceTweenId = LeanTween.moveLocalY(promptRect.gameObject,
            promptRect.localPosition.y + bounceAmount,
            bounceSpeed)
            .setEaseInOutSine()
            .setLoopPingPong()
            .id;

        canAdvance = true;
    }

    void HidePrompt(System.Action onComplete)
    {
        canAdvance = false;
        LeanTween.cancel(bounceTweenId);

        LeanTween.alphaCanvas(promptCanvasGroup, 0f, fadeDuration)
            .setOnComplete(() =>
            {
                promptButton.SetActive(false);
                onComplete?.Invoke();
            });
    }

    void Advance()
    {
        // Som de clique
        if (nextSound != null)
        {
            SoundManagerSO.PlaySoundFXClip(nextSound, transform.position, 1f);
        }

        HidePrompt(() =>
        {
            currentIndex++;
            if (currentIndex >= introImages.Length)
            {
                EndSequence();
            }
            else
            {
                ShowImage(currentIndex);
            }
        });
    }

    void EndSequence()
    {
        PlayerPrefs.SetInt("IntroSeen", 1);
        PlayerPrefs.Save();
        EnableGameplay();
        gameObject.SetActive(false);
    }

    void DisableGameplay()
    {
        foreach (var script in scriptsToDisable)
        {
            script.enabled = false;
        }
    }

    void EnableGameplay()
    {
        foreach (var script in scriptsToDisable)
        {
            script.enabled = true;
        }
    }
}
