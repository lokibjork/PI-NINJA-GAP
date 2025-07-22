using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro; // Certifique-se de ter isso para TextMeshPro

public class GameOverScreenManager : MonoBehaviour
{
    // Padr�o Singleton
    public static GameOverScreenManager Instance { get; private set; }

    [Header("Tela de Game Over")]
    public GameObject gameOverCanvas; // Esta refer�ncia ser� re-atribu�da via c�digo
    public CanvasGroup gameOverPanelCanvasGroup; // Esta refer�ncia ser� re-atribu�da via c�digo
    public TMP_Text gameOverMessageText; // Esta refer�ncia ser� re-atribu�da via c�digo
    public float fadeInDuration = 1f;
    [TextArea(3, 5)]
    public string[] sarcasticGameOverMessages = new string[]
    {
        "��o foi como planejado, n�?",
        "Parab�ns! Voc� alcan�ou o objetivo... morrer.",
        "Incr�vel! Aplausos...",
        "N�o se preocupe, acontece com os melhores... e com voc�.",
        "Talvez jardinagem seja uma carreira melhor.",
        "Spoiler: voc� n�o sobreviveu.",
        "Aposto que sua m�e est� orgulhosa.",
        "Tenta n�o morrer da pr�xima vez por favor.",
        "Voc� fez o seu melhor. O seu melhor n�o foi o suficiente.",
        "Esse � o seu melhor? kkkkkkkk"
    };
    public float textRevealSpeed = 50f; // Caracteres por segundo

    // NOVO: Campo para o som de digita��o, agora um AudioClip simples
    [Header("Som de Digita��o")]
    [Tooltip("O clipe de �udio para cada caractere digitado, tocado via SoundManagerSO.")]
    public AudioClip typewriterSoundClip; // Apenas o AudioClip, o SoundManager gerencia o AudioSource
    [Tooltip("Volume do som de digita��o.")]
    public float typewriterVolume = 0.5f; // Ajuste o volume aqui
    [Tooltip("Atraso m�nimo entre os sons de digita��o para evitar sobreposi��o excessiva.")]
    public float soundPlayDelay = 0.05f; // Ajuste para o seu gosto
    private float lastSoundTime = 0f;

    [Header("Texto de Reiniciar Abaixo da Fala")]
    public GameObject restartTextObject; // Esta refer�ncia ser� re-atribu�da via c�digo
    public CanvasGroup restartTextCanvasGroup; // Esta refer�ncia ser� re-atribu�da via c�digo
    public float restartTextDelay = 1.5f; // Tempo de atraso antes de mostrar o texto
    public float restartTextFadeInDuration = 1f; // Dura��o do fade in do texto de reiniciar

    private bool isTyping = false;
    private Coroutine typewriterCoroutine;
    private Coroutine fadeInPanelCoroutine;
    private Coroutine fadeInRestartTextCoroutine;

    void Awake()
    {
        // Implementa��o do Singleton: Garante que s� h� uma inst�ncia e que ela persiste
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"GameOverScreenManager: Destruindo inst�ncia duplicada de '{gameObject.name}'.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Torna este GameObject persistente entre as cenas

        // Subscreve os eventos
        Player.DamageHandler.OnPlayerDeath += ShowGameOverScreen;
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("GameOverScreenManager: Awake conclu�do. Inst�ncia configurada e DontDestroyOnLoad aplicado.");

        // Chama a fun��o para buscar as refer�ncias do Canvas no Awake
        InitializeCanvasReferences();
    }

    void OnDestroy()
    {
        Debug.Log("GameOverScreenManager: OnDestroy chamado. Eventos dessubscritos e corrotinas paradas.");
        Player.DamageHandler.OnPlayerDeath -= ShowGameOverScreen;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StopAllCoroutines();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GameOverScreenManager: Cena '{scene.name}' carregada. Reinicializando refer�ncias e UI.");
        InitializeCanvasReferences();

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
            if (gameOverPanelCanvasGroup != null) gameOverPanelCanvasGroup.alpha = 0f;
            if (gameOverMessageText != null) gameOverMessageText.text = "";
            if (restartTextCanvasGroup != null) restartTextCanvasGroup.alpha = 0f;
            if (restartTextObject != null) restartTextObject.SetActive(true);
            isTyping = false;
        }
        else
        {
            Debug.LogError("GameOverScreenManager: gameOverCanvas � NULO em OnSceneLoaded AP�S tentar inicializar refer�ncias. O Canvas n�o foi encontrado ou est� inacess�vel.");
        }
    }

    private void InitializeCanvasReferences()
    {
        Debug.Log("GameOverScreenManager: InitializeCanvasReferences() chamado para buscar refer�ncias.");

        if (gameOverCanvas == null)
        {
            gameOverCanvas = transform.Find("GameOverCanvasName")?.gameObject;
            if (gameOverCanvas == null)
            {
                gameOverCanvas = GameObject.FindWithTag("GameOverCanvas");
            }
        }

        if (gameOverCanvas == null)
        {
            Debug.LogError("GameOverScreenManager: N�o foi poss�vel encontrar o GameObject do Canvas de Game Over! Atribua-o manualmente no Inspector ou verifique o nome/tag.");
            return;
        }

        gameOverPanelCanvasGroup = gameOverCanvas.GetComponentInChildren<CanvasGroup>(true);
        gameOverMessageText = gameOverCanvas.GetComponentInChildren<TMP_Text>(true);

        if (gameOverCanvas.transform.Find("GameOverPanel/RestartTextObject") != null)
        {
            restartTextObject = gameOverCanvas.transform.Find("GameOverPanel/RestartTextObject").gameObject;
            restartTextCanvasGroup = restartTextObject.GetComponent<CanvasGroup>();
        }
        else
        {
            TMP_Text[] texts = gameOverCanvas.GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text text in texts)
            {
                if (text.gameObject.name.Contains("Restart"))
                {
                    restartTextObject = text.gameObject;
                    restartTextCanvasGroup = text.GetComponent<CanvasGroup>();
                    break;
                }
            }
        }

        Debug.Log($"GameOverScreenManager: gameOverCanvas encontrado: {(gameOverCanvas != null ? gameOverCanvas.name : "NULO")}");
        Debug.Log($"GameOverScreenManager: gameOverPanelCanvasGroup encontrado: {(gameOverPanelCanvasGroup != null ? gameOverPanelCanvasGroup.name : "NULO")}");
        Debug.Log($"GameOverScreenManager: gameOverMessageText encontrado: {(gameOverMessageText != null ? gameOverMessageText.name : "NULO")}");
        Debug.Log($"GameOverScreenManager: restartTextObject encontrado: {(restartTextObject != null ? restartTextObject.name : "NULO")}");
        Debug.Log($"GameOverScreenManager: restartTextCanvasGroup encontrado: {(restartTextCanvasGroup != null ? restartTextCanvasGroup.name : "NULO")}");

        if (gameOverPanelCanvasGroup == null || gameOverMessageText == null || restartTextObject == null || restartTextCanvasGroup == null)
        {
            Debug.LogWarning("GameOverScreenManager: Algumas refer�ncias do Canvas ainda s�o NULAS ap�s a busca. Verifique a hierarquia e nomes!");
        }
    }

    void ShowGameOverScreen()
    {
        Debug.Log("GameOverScreenManager: ShowGameOverScreen chamado.");
        Debug.Log($"GameOverScreenManager: Refer�ncia de gameOverCanvas ao tentar mostrar: {(gameOverCanvas != null ? gameOverCanvas.name : "NULO")}");

        if (gameOverCanvas != null)
        {
            if (gameOverPanelCanvasGroup != null) gameOverPanelCanvasGroup.alpha = 0f;
            if (gameOverMessageText != null) gameOverMessageText.text = "";
            if (restartTextCanvasGroup != null) restartTextCanvasGroup.alpha = 0f;
            if (restartTextObject != null) restartTextObject.SetActive(true);
            isTyping = false;

            gameOverCanvas.SetActive(true);
            Debug.Log("GameOverScreenManager: Canvas ativado com sucesso.");

            if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
            if (fadeInPanelCoroutine != null) StopCoroutine(fadeInPanelCoroutine);
            if (fadeInRestartTextCoroutine != null) StopCoroutine(fadeInRestartTextCoroutine);

            fadeInPanelCoroutine = StartCoroutine(FadeInGameOver());
            typewriterCoroutine = StartCoroutine(TypewriterEffect(gameOverMessageText, GetRandomGameOverMessage()));
            fadeInRestartTextCoroutine = StartCoroutine(FadeInRestartText());
        }
        else
        {
            Debug.LogError("GameOverScreenManager: gameOverCanvas � NULO ao tentar mostrar a tela de Game Over! A refer�ncia n�o foi estabelecida.");
        }
    }

    IEnumerator FadeInGameOver()
    {
        float timer = 0f;
        if (gameOverPanelCanvasGroup == null) { Debug.LogWarning("GameOverScreenManager: FadeInGameOver: Canvas Group do painel � nulo! Saindo da corrotina."); yield break; }

        gameOverPanelCanvasGroup.alpha = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            if (gameOverPanelCanvasGroup == null) { Debug.LogWarning("GameOverScreenManager: FadeInGameOver: Canvas Group do painel foi destru�do durante o fade! Saindo."); yield break; }
            gameOverPanelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
            yield return null;
        }
        if (gameOverPanelCanvasGroup != null) gameOverPanelCanvasGroup.alpha = 1f;
        Debug.Log("GameOverScreenManager: FadeInGameOver conclu�do.");
    }

    IEnumerator TypewriterEffect(TMP_Text textComponent, string textToType)
    {
        if (textComponent == null) { Debug.LogWarning("GameOverScreenManager: TypewriterEffect: Text Component � nulo! Saindo da corrotina."); yield break; }

        textComponent.text = "";
        float t = 0;
        int charIndex = 0;
        isTyping = true;
        Debug.Log("GameOverScreenManager: TypewriterEffect iniciado.");

        while (charIndex < textToType.Length)
        {
            if (textComponent == null) { Debug.LogWarning("GameOverScreenManager: TypewriterEffect: Text Component foi destru�do durante a digita��o! Saindo."); yield break; }
            t += Time.deltaTime * textRevealSpeed;
            int newCharIndex = Mathf.FloorToInt(t);
            newCharIndex = Mathf.Clamp(newCharIndex, 0, textToType.Length);

            // NOVO: Toca o som a cada nova letra, usando o SoundManagerSO
            if (newCharIndex > charIndex && typewriterSoundClip != null)
            {
                if (Time.time - lastSoundTime >= soundPlayDelay)
                {
                    // Usa SoundManagerSO para tocar o som na posi��o do GameObject do Canvas
                    SoundManagerSO.PlaySoundFXClip(typewriterSoundClip, transform.position, typewriterVolume);
                    lastSoundTime = Time.time;
                }
            }

            charIndex = newCharIndex; // Atualiza o charIndex ap�s a verifica��o do som
            textComponent.text = textToType.Substring(0, charIndex);
            yield return null;
        }
        if (textComponent != null) isTyping = false;
        Debug.Log("GameOverScreenManager: TypewriterEffect conclu�do.");
    }

    string GetRandomGameOverMessage()
    {
        if (sarcasticGameOverMessages == null || sarcasticGameOverMessages.Length == 0)
        {
            Debug.LogWarning("GameOverScreenManager: Nenhuma mensagem de Game Over definida! Usando padr�o.");
            return "Game Over";
        }
        return sarcasticGameOverMessages[Random.Range(0, sarcasticGameOverMessages.Length)];
    }

    IEnumerator FadeInRestartText()
    {
        if (restartTextCanvasGroup == null) { Debug.LogWarning("GameOverScreenManager: FadeInRestartText: Canvas Group do texto de reiniciar � nulo! Saindo da corrotina."); yield break; }

        yield return new WaitForSeconds(restartTextDelay);

        float timer = 0f;
        if (restartTextCanvasGroup == null) { Debug.LogWarning("GameOverScreenManager: FadeInRestartText: Canvas Group do texto de reiniciar foi destru�do antes do fade! Saindo."); yield break; }
        restartTextCanvasGroup.alpha = 0f;
        if (restartTextObject != null) restartTextObject.SetActive(true);
        Debug.Log("GameOverScreenManager: FadeInRestartText iniciado.");

        while (timer < restartTextFadeInDuration)
        {
            timer += Time.deltaTime;
            if (restartTextCanvasGroup == null) { Debug.LogWarning("GameOverScreenManager: FadeInRestartText: Canvas Group do texto de reiniciar foi destru�do durante o fade! Saindo."); yield break; }
            restartTextCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / restartTextFadeInDuration);
            yield return null;
        }
        if (restartTextCanvasGroup != null) restartTextCanvasGroup.alpha = 1f;
        Debug.Log("GameOverScreenManager: FadeInRestartText conclu�do.");
    }

    public void RestartGame()
    {
        Debug.Log("GameOverScreenManager: RestartGame chamado. Preparando para recarregar cena.");

        if (typewriterCoroutine != null) { StopCoroutine(typewriterCoroutine); typewriterCoroutine = null; }
        if (fadeInPanelCoroutine != null) { StopCoroutine(fadeInPanelCoroutine); fadeInPanelCoroutine = null; }
        if (fadeInRestartTextCoroutine != null) { StopCoroutine(fadeInRestartTextCoroutine); fadeInRestartTextCoroutine = null; }

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
            if (gameOverPanelCanvasGroup != null) gameOverPanelCanvasGroup.alpha = 0f;
            if (gameOverMessageText != null) gameOverMessageText.text = "";
            if (restartTextCanvasGroup != null) restartTextCanvasGroup.alpha = 0f;
            Debug.Log("GameOverScreenManager: gameOverCanvas desativado e alpha resetado antes do carregamento da cena.");
        }
        else
        {
            Debug.LogWarning("GameOverScreenManager: gameOverCanvas � nulo em RestartGame! N�o foi poss�vel desativar (mas a cena ser� recarregada).");
        }

        isTyping = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("GameOverScreenManager: Carregamento da cena iniciado.");
    }

    void Update()
    {
        if (gameOverCanvas != null && gameOverCanvas.activeSelf && !isTyping && Input.GetButtonDown("Fire1"))
        {
            RestartGame();
        }
    }
}