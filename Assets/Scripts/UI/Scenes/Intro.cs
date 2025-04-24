using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

public class Intro : MonoBehaviour
{
    public GameObject promptButton;
    private RectTransform promptRect;
    private CanvasGroup promptCanvasGroup;

    public float fadeDuration = 0.5f;
    public float bounceAmount = 10f;
    public float bounceSpeed = 0.5f;

    private int currentIndex = 0;
    private bool canAdvance = false;
    private int bounceTweenId;
    public UnityEvent onIntroSequenceEnded;

    // Scripts a serem desativados durante a intro
    public MonoBehaviour[] scriptsToDisable;

    [Header("Sequ�ncia da Intro")]
    public List<GameObject> introSequenceObjects; // Lista de GameObjects a serem controlados na sequ�ncia
    public float[] displayTimes; // Array de tempos (em segundos) para exibir cada objeto

    [Header("Anima��es de Sa�da dos Assets da Cena")]
    public Animator[] sceneAssetsAnimators; // Array para referenciar os animators dos assets da cena
    public string isPressedBoolName = "IsPressed"; // Nome da booleana no Animator
    public float animationOutDuration = 1f; // Dura��o ESTIMADA das anima��es de sa�da (usado como fallback)

    [Header("PlayerPrefs")]
    public string hasSeenIntroKey = "HasSeenIntro"; // Chave para salvar no PlayerPrefs
    public bool playIntroOnce = true; // Define se a intro deve tocar apenas uma vez

    void Awake()
    {
        // Verifica se a intro j� foi vista
        if (playIntroOnce && PlayerPrefs.GetInt(hasSeenIntroKey, 0) == 1)
        {
            // Se j� viu, desativa este GameObject e habilita o gameplay diretamente
            EnableGameplay();
            gameObject.SetActive(false);
            return; // Sai do Awake para n�o executar o resto da l�gica da intro
        }

        promptCanvasGroup = promptButton.GetComponent<CanvasGroup>();
        promptRect = promptButton.GetComponent<RectTransform>();

        promptButton.SetActive(false);
        DisableGameplay();

        // Inicializa o estado dos objetos da sequ�ncia
        foreach (var obj in introSequenceObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false); // Desativa todos por padr�o
            }
        }

        // Garante que a booleana IsPressed esteja desativada no in�cio
        foreach (var animator in sceneAssetsAnimators)
        {
            if (animator != null)
            {
                animator.SetBool(isPressedBoolName, false);
            }
            else
            {
                Debug.LogWarning("Um dos animators de assets da cena n�o foi atribu�do!");
            }
        }

        // Inicia a sequ�ncia mostrando o primeiro objeto (se houver)
        if (introSequenceObjects.Count > 0)
        {
            ShowObject(0);
        }
        else
        {
            ShowPrompt(); // Se n�o houver objetos na sequ�ncia, mostra o prompt diretamente
        }

        // Valida��o do array de tempos
        if (displayTimes.Length != introSequenceObjects.Count)
        {
            Debug.LogError("O n�mero de tempos em 'Display Times' n�o corresponde ao n�mero de objetos em 'Intro Sequence Objects'!");
        }
    }

    void Update()
    {
        if (canAdvance && Input.GetButtonDown("Fire1"))
        {
            TriggerOutAnimations();
            float delayTime = animationOutDuration; // Tempo padr�o

            // Verifica se h� um tempo espec�fico definido para a cena atual
            if (currentIndex < displayTimes.Length && displayTimes[currentIndex] > 0)
            {
                delayTime = displayTimes[currentIndex];
            }

            // Avan�ar para o pr�ximo objeto ap�s o delay
            Invoke("AdvanceSequence", delayTime);
            canAdvance = false; // Desativa o avan�o m�ltiplo com um �nico pressionamento
        }
    }

    void ShowObject(int index)
    {
        if (index >= 0 && index < introSequenceObjects.Count && introSequenceObjects[index] != null)
        {
            introSequenceObjects[index].SetActive(true);
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
        LeanTween.cancel(bounceTweenId);

        LeanTween.alphaCanvas(promptCanvasGroup, 0f, fadeDuration)
            .setOnComplete(() =>
            {
                promptButton.SetActive(false);
                onComplete?.Invoke();
            });
    }

    void AdvanceSequence()
    {
        // Desativa o objeto atual
        if (currentIndex >= 0 && currentIndex < introSequenceObjects.Count && introSequenceObjects[currentIndex] != null)
        {
            introSequenceObjects[currentIndex].SetActive(false);
        }

        currentIndex++;
        if (currentIndex >= introSequenceObjects.Count)
        {
            EndSequence();
        }
        else
        {
            ShowObject(currentIndex);
        }
    }

    void EndSequence()
    {
        // Salva que a intro foi vista
        if (playIntroOnce)
        {
            PlayerPrefs.SetInt(hasSeenIntroKey, 1);
            PlayerPrefs.Save();
        }
        EnableGameplay();
        gameObject.SetActive(false); // ou transi��o pra pr�xima cena
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

    void TriggerOutAnimations()
    {
        // Ativa a booleana IsPressed em todos os animators dos assets da cena
        foreach (var animator in sceneAssetsAnimators)
        {
            if (animator != null)
            {
                animator.SetBool(isPressedBoolName, true);
            }
        }
        HidePrompt(null); // Oculta o prompt imediatamente ao pressionar o bot�o
    }
}