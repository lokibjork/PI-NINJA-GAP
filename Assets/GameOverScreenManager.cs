using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro; // Certifique-se de ter isso para TextMeshPro

public class GameOverScreenManager : MonoBehaviour
{
    // Padrão Singleton
    public static GameOverScreenManager Instance { get; private set; }

    [Header("Tela de Game Over")]
    public GameObject gameOverCanvas; // Esta referência será re-atribuída via código
    public CanvasGroup gameOverPanelCanvasGroup; // Esta referência será re-atribuída via código
    public TMP_Text gameOverMessageText; // Esta referência será re-atribuída via código
    public float fadeInDuration = 1f;
    [TextArea(3, 5)]
    public string[] sarcasticGameOverMessages = new string[]
    {
        "Ñão foi como planejado, né?",
        "Parabéns! Você alcançou o objetivo... morrer.",
        "Incrível! Aplausos...",
        "Não se preocupe, acontece com os melhores... e com você.",
        "Talvez jardinagem seja uma carreira melhor.",
        "Spoiler: você não sobreviveu.",
        "Aposto que sua mãe está orgulhosa.",
        "Tenta não morrer da próxima vez por favor.",
        "Você fez o seu melhor. O seu melhor não foi o suficiente.",
        "Esse é o seu melhor? kkkkkkkk"
    };
    public float textRevealSpeed = 50f; // Caracteres por segundo

    [Header("Texto de Reiniciar Abaixo da Fala")]
    public GameObject restartTextObject; // Esta referência será re-atribuída via código
    public CanvasGroup restartTextCanvasGroup; // Esta referência será re-atribuída via código
    public float restartTextDelay = 1.5f; // Tempo de atraso antes de mostrar o texto
    public float restartTextFadeInDuration = 1f; // Duração do fade in do texto de reiniciar

    private bool isTyping = false;
    private Coroutine typewriterCoroutine;
    private Coroutine fadeInPanelCoroutine;
    private Coroutine fadeInRestartTextCoroutine;

    void Awake()
    {
        // Implementação do Singleton: Garante que só há uma instância e que ela persiste
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"GameOverScreenManager: Destruindo instância duplicada de '{gameObject.name}'.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Torna este GameObject persistente entre as cenas**

        // Subscreve os eventos
        Player.DamageHandler.OnPlayerDeath += ShowGameOverScreen;
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("GameOverScreenManager: Awake concluído. Instância configurada e DontDestroyOnLoad aplicado.");

        // Chama a função para buscar as referências do Canvas no Awake
        InitializeCanvasReferences();
    }

    void OnDestroy()
    {
        Debug.Log("GameOverScreenManager: OnDestroy chamado. Eventos dessubscritos e corrotinas paradas.");
        Player.DamageHandler.OnPlayerDeath -= ShowGameOverScreen;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StopAllCoroutines();
    }

    // Este método é chamado toda vez que uma nova cena é carregada
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GameOverScreenManager: Cena '{scene.name}' carregada. Reinicializando referências e UI.");
        // Re-chama a função para buscar as referências do Canvas a cada carregamento de cena
        InitializeCanvasReferences();

        // Garante que o Canvas de Game Over esteja desativado e resetado ao iniciar cada cena
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
            if (gameOverPanelCanvasGroup != null) gameOverPanelCanvasGroup.alpha = 0f;
            if (gameOverMessageText != null) gameOverMessageText.text = "";
            if (restartTextCanvasGroup != null) restartTextCanvasGroup.alpha = 0f;
            if (restartTextObject != null) restartTextObject.SetActive(true); // Garante que o objeto do texto esteja ativo
            isTyping = false; // Garante que a flag de digitação esteja resetada
        }
        else
        {
            Debug.LogError("GameOverScreenManager: gameOverCanvas é NULO em OnSceneLoaded APÓS tentar inicializar referências. O Canvas não foi encontrado ou está inacessível.");
        }
    }

    // NOVA FUNÇÃO: Busca e atribui todas as referências do Canvas e seus filhos
    private void InitializeCanvasReferences()
    {
        Debug.Log("GameOverScreenManager: InitializeCanvasReferences() chamado para buscar referências.");

        // O seu Canvas de Game Over DEVE ser um filho direto do GameObject onde este script está (o _GameManager).
        // Ou, se ele estiver em outro lugar na cena, você pode tentar encontrá-lo com FindWithTag,
        // mas a abordagem de filho é mais robusta para Singletons com DontDestroyOnLoad.

        // Tenta encontrar o Canvas de Game Over como um filho direto
        if (gameOverCanvas == null) // Apenas tenta encontrar se já não tiver sido atribuído (ex: via inspector na primeira vez)
        {
            // Se o Canvas for filho direto e persistente, ele já estará aqui
            gameOverCanvas = transform.Find("GameOverCanvasName")?.gameObject; // Substitua "GameOverCanvasName" pelo NOME EXATO do seu GameObject Canvas
            if (gameOverCanvas == null)
            {
                // Alternativa: Se o Canvas pode não ser filho direto, mas tem uma Tag
                gameOverCanvas = GameObject.FindWithTag("GameOverCanvas"); // Certifique-se de que seu Canvas tem a tag "GameOverCanvas"
            }
        }

        if (gameOverCanvas == null)
        {
            Debug.LogError("GameOverScreenManager: Não foi possível encontrar o GameObject do Canvas de Game Over! Atribua-o manualmente no Inspector ou verifique o nome/tag.");
            // Não podemos prosseguir se o Canvas principal não for encontrado
            return;
        }

        // Tenta encontrar os CanvasGroups e TextMeshPro dentro do Canvas encontrado
        // O `true` no GetComponentInChildren é para procurar também em objetos inativos
        gameOverPanelCanvasGroup = gameOverCanvas.GetComponentInChildren<CanvasGroup>(true);
        gameOverMessageText = gameOverCanvas.GetComponentInChildren<TMP_Text>(true);

        // Para o restartTextObject, pode ser necessário uma busca mais específica se houver vários TMP_Text
        if (gameOverCanvas.transform.Find("GameOverPanel/RestartTextObject") != null) // Ajuste o caminho se necessário
        {
            restartTextObject = gameOverCanvas.transform.Find("GameOverPanel/RestartTextObject").gameObject;
            restartTextCanvasGroup = restartTextObject.GetComponent<CanvasGroup>();
        }
        else
        {
            // Alternativa se o caminho direto não funcionar ou se o nome for variável
            TMP_Text[] texts = gameOverCanvas.GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text text in texts)
            {
                // Adapte esta condição para identificar o seu texto de reiniciar (ex: pelo nome do GameObject ou pelo próprio texto)
                if (text.gameObject.name.Contains("Restart"))
                {
                    restartTextObject = text.gameObject;
                    restartTextCanvasGroup = text.GetComponent<CanvasGroup>();
                    break;
                }
            }
        }


        // Logs de verificação das referências após a busca
        Debug.Log($"GameOverScreenManager: gameOverCanvas encontrado: {(gameOverCanvas != null ? gameOverCanvas.name : "NULO")}");
        Debug.Log($"GameOverScreenManager: gameOverPanelCanvasGroup encontrado: {(gameOverPanelCanvasGroup != null ? gameOverPanelCanvasGroup.name : "NULO")}");
        Debug.Log($"GameOverScreenManager: gameOverMessageText encontrado: {(gameOverMessageText != null ? gameOverMessageText.name : "NULO")}");
        Debug.Log($"GameOverScreenManager: restartTextObject encontrado: {(restartTextObject != null ? restartTextObject.name : "NULO")}");
        Debug.Log($"GameOverScreenManager: restartTextCanvasGroup encontrado: {(restartTextCanvasGroup != null ? restartTextCanvasGroup.name : "NULO")}");

        // Se alguma das referências ainda for nula aqui, o problema é na estrutura da UI ou nos nomes.
        if (gameOverPanelCanvasGroup == null || gameOverMessageText == null || restartTextObject == null || restartTextCanvasGroup == null)
        {
            Debug.LogWarning("GameOverScreenManager: Algumas referências do Canvas ainda são NULAS após a busca. Verifique a hierarquia e nomes!");
        }
    }


    void ShowGameOverScreen()
    {
        Debug.Log("GameOverScreenManager: ShowGameOverScreen chamado.");
        Debug.Log($"GameOverScreenManager: Referência de gameOverCanvas ao tentar mostrar: {(gameOverCanvas != null ? gameOverCanvas.name : "NULO")}");

        if (gameOverCanvas != null)
        {
            // Resetamos o estado novamente para garantir que as animações comecem do zero
            if (gameOverPanelCanvasGroup != null) gameOverPanelCanvasGroup.alpha = 0f;
            if (gameOverMessageText != null) gameOverMessageText.text = "";
            if (restartTextCanvasGroup != null) restartTextCanvasGroup.alpha = 0f;
            if (restartTextObject != null) restartTextObject.SetActive(true);
            isTyping = false;

            gameOverCanvas.SetActive(true);
            Debug.Log("GameOverScreenManager: Canvas ativado com sucesso.");

            // Parar corrotinas anteriores antes de iniciar novas
            if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
            if (fadeInPanelCoroutine != null) StopCoroutine(fadeInPanelCoroutine);
            if (fadeInRestartTextCoroutine != null) StopCoroutine(fadeInRestartTextCoroutine);

            fadeInPanelCoroutine = StartCoroutine(FadeInGameOver());
            typewriterCoroutine = StartCoroutine(TypewriterEffect(gameOverMessageText, GetRandomGameOverMessage()));
            fadeInRestartTextCoroutine = StartCoroutine(FadeInRestartText());
        }
        else
        {
            Debug.LogError("GameOverScreenManager: gameOverCanvas é NULO ao tentar mostrar a tela de Game Over! A referência não foi estabelecida.");
        }
    }

    IEnumerator FadeInGameOver()
    {
        float timer = 0f;
        if (gameOverPanelCanvasGroup == null) { Debug.LogWarning("GameOverScreenManager: FadeInGameOver: Canvas Group do painel é nulo! Saindo da corrotina."); yield break; }

        gameOverPanelCanvasGroup.alpha = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            if (gameOverPanelCanvasGroup == null) { Debug.LogWarning("GameOverScreenManager: FadeInGameOver: Canvas Group do painel foi destruído durante o fade! Saindo."); yield break; }
            gameOverPanelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
            yield return null;
        }
        if (gameOverPanelCanvasGroup != null) gameOverPanelCanvasGroup.alpha = 1f;
        Debug.Log("GameOverScreenManager: FadeInGameOver concluído.");
    }

    IEnumerator TypewriterEffect(TMP_Text textComponent, string textToType)
    {
        if (textComponent == null) { Debug.LogWarning("GameOverScreenManager: TypewriterEffect: Text Component é nulo! Saindo da corrotina."); yield break; }

        textComponent.text = "";
        float t = 0;
        int charIndex = 0;
        isTyping = true;
        Debug.Log("GameOverScreenManager: TypewriterEffect iniciado.");

        while (charIndex < textToType.Length)
        {
            if (textComponent == null) { Debug.LogWarning("GameOverScreenManager: TypewriterEffect: Text Component foi destruído durante a digitação! Saindo."); yield break; }
            t += Time.deltaTime * textRevealSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);
            textComponent.text = textToType.Substring(0, charIndex);
            yield return null;
        }
        if (textComponent != null) isTyping = false;
        Debug.Log("GameOverScreenManager: TypewriterEffect concluído.");
    }

    string GetRandomGameOverMessage()
    {
        if (sarcasticGameOverMessages == null || sarcasticGameOverMessages.Length == 0)
        {
            Debug.LogWarning("GameOverScreenManager: Nenhuma mensagem de Game Over definida! Usando padrão.");
            return "Game Over";
        }
        return sarcasticGameOverMessages[Random.Range(0, sarcasticGameOverMessages.Length)];
    }

    IEnumerator FadeInRestartText()
    {
        if (restartTextCanvasGroup == null) { Debug.LogWarning("GameOverScreenManager: FadeInRestartText: Canvas Group do texto de reiniciar é nulo! Saindo da corrotina."); yield break; }

        yield return new WaitForSeconds(restartTextDelay);

        float timer = 0f;
        if (restartTextCanvasGroup == null) { Debug.LogWarning("GameOverScreenManager: FadeInRestartText: Canvas Group do texto de reiniciar foi destruído antes do fade! Saindo."); yield break; }
        restartTextCanvasGroup.alpha = 0f;
        if (restartTextObject != null) restartTextObject.SetActive(true);
        Debug.Log("GameOverScreenManager: FadeInRestartText iniciado.");

        while (timer < restartTextFadeInDuration)
        {
            timer += Time.deltaTime;
            if (restartTextCanvasGroup == null) { Debug.LogWarning("GameOverScreenManager: FadeInRestartText: Canvas Group do texto de reiniciar foi destruído durante o fade! Saindo."); yield break; }
            restartTextCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / restartTextFadeInDuration);
            yield return null;
        }
        if (restartTextCanvasGroup != null) restartTextCanvasGroup.alpha = 1f;
        Debug.Log("GameOverScreenManager: FadeInRestartText concluído.");
    }

    public void RestartGame()
    {
        Debug.Log("GameOverScreenManager: RestartGame chamado. Preparando para recarregar cena.");

        // Parar todas as corrotinas ativas
        if (typewriterCoroutine != null) { StopCoroutine(typewriterCoroutine); typewriterCoroutine = null; }
        if (fadeInPanelCoroutine != null) { StopCoroutine(fadeInPanelCoroutine); fadeInPanelCoroutine = null; }
        if (fadeInRestartTextCoroutine != null) { StopCoroutine(fadeInRestartTextCoroutine); fadeInRestartTextCoroutine = null; }

        // Desativa o Canvas de Game Over e reseta alpha ANTES de carregar a cena
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
            Debug.LogWarning("GameOverScreenManager: gameOverCanvas é nulo em RestartGame! Não foi possível desativar (mas a cena será recarregada).");
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