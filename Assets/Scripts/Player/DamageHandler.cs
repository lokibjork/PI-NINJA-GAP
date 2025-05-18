using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class DamageHandler : MonoBehaviour
{
    public Player.PlayerData playerData;
    private bool isInvulnerable = false;
    public float invulnerabilityTime = 0.2f;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public AudioClip _playerHit;
    public ScreenShaker screenShaker;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.1f;

    [Header("Deleção na Morte")]
    public GameObject[] objectsToDeleteOnDeath; // Array de GameObjects para deletar

    [Header("Desativação na Morte")]
    public MonoBehaviour[] scriptsToDisableOnDeath;

    [Header("Efeito de Morte Sarcástica")]
    public GameObject tomatoChunkPrefab;
    public GameObject bloodParticlePrefab;
    public int numberOfTomatoChunks = 15;
    public float explosionForce = 5f;
    public float deathEffectDuration = 2f;

    [Header("Tela de Game Over")]
    public GameObject gameOverCanvas;
    public CanvasGroup gameOverPanelCanvasGroup;
    public TMP_Text gameOverMessageText;
    public float fadeInDuration = 1f;
    [TextArea(3, 5)]
    public string[] sarcasticGameOverMessages = new string[]
    {
        "Bem, isso não correu como planejado, não é?",
        "Parabéns! Você alcançou o objetivo... de morrer.",
        "Que performance! Aplausos... virtuais.",
        "Não se preocupe, acontece com os melhores... e com você.",
        "Talvez a jardinagem seja uma carreira melhor.",
        "Spoiler: você não sobreviveu.",
        "Aposto que sua mãe está orgulhosa.",
        "Game Over. Tente não morrer da próxima vez (sem promessas).",
        "Você fez o seu melhor. O seu melhor não foi o suficiente.",
        "Pelo menos os tomates tiveram um final explosivo."
    };
    public float textRevealSpeed = 50f; // Caracteres por segundo

    [Header("Texto de Reiniciar Abaixo da Fala")]
    public GameObject restartTextObject; // GameObject contendo o texto de reiniciar
    public CanvasGroup restartTextCanvasGroup; // Canvas Group do texto de reiniciar
    public float restartTextDelay = 0.5f; // Pequeno atraso após a digitação
    public float restartTextFadeInDuration = 1f; // Duração do fade in do texto de reiniciar

    private bool isDead = false; // Flag para evitar chamadas múltiplas de Die()
    private bool isTyping = false; // Flag para indicar se o texto está sendo digitado

    // Evento estático para notificar outros scripts sobre a morte do jogador
    public static event System.Action OnPlayerDeath;

    void Start()
    {
        if (playerData == null)
        {
            playerData = GetComponent<Player.PlayerData>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("Game Over Canvas não atribuído ao DamageHandler!");
        }

        if (gameOverPanelCanvasGroup != null)
        {
            gameOverPanelCanvasGroup.alpha = 0f;
        }
        else if (gameOverCanvas != null)
        {
            gameOverPanelCanvasGroup = gameOverCanvas.GetComponentInChildren<CanvasGroup>();
            if (gameOverPanelCanvasGroup == null)
            {
                Debug.LogWarning("Canvas Group não encontrado no Panel dentro do Game Over Canvas!");
            }
            else
            {
                gameOverPanelCanvasGroup.alpha = 0f; // Garante que o alpha comece em 0 se encontrado tardiamente
            }
        }

        if (gameOverMessageText != null)
        {
            gameOverMessageText.text = ""; // Inicializa o texto vazio
        }

        // Inicializa o texto de reiniciar invisível
        if (restartTextCanvasGroup != null && restartTextObject != null)
        {
            restartTextCanvasGroup.alpha = 0f;
            restartTextObject.SetActive(true);
        }
        else if (restartTextObject != null)
        {
            restartTextCanvasGroup = restartTextObject.GetComponent<CanvasGroup>();
            if (restartTextCanvasGroup != null)
            {
                restartTextCanvasGroup.alpha = 0f;
                restartTextObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Canvas Group não encontrado no objeto de texto de reiniciar!");
            }
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        if (isInvulnerable || isDead) return;

        playerData.currentHealth -= damage;
        playerData.healthBar.SetHealth(playerData.currentHealth);
        SoundManagerSO.PlaySoundFXClip(_playerHit, transform.position, 1f);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
        }

        if (screenShaker != null)
        {
            screenShaker.Shake(-knockbackDirection.normalized * shakeIntensity);
        }

        StartCoroutine(InvulnerabilityCoroutine());

        if (playerData.currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;

        if (spriteRenderer != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < invulnerabilityTime)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.1f);
                elapsedTime += 0.2f;
            }
            spriteRenderer.color = Color.white;
        }

        isInvulnerable = false;
    }

    public void Heal(int amount)
    {
        playerData.currentHealth += amount;
        if (playerData.currentHealth > playerData.maxHealth)
            playerData.currentHealth = playerData.maxHealth;

        playerData.healthBar.SetHealth(playerData.currentHealth);
        Debug.Log("Recuperei " + amount + " de vida. Vida atual: " + playerData.currentHealth);
    }

    public void Die()
    {
        if (isDead) return; // Evita chamadas múltiplas
        isDead = true;

        Debug.Log("Player morreu! Iniciando efeito de morte...");

        // Deleta os objetos listados
        if (objectsToDeleteOnDeath != null)
        {
            foreach (GameObject objToDelete in objectsToDeleteOnDeath)
            {
                if (objToDelete != null)
                {
                    Destroy(objToDelete);
                    Debug.Log($"Objeto deletado: {objToDelete.name}");
                }
            }
        }

        // Desativa os scripts listados
        if (scriptsToDisableOnDeath != null)
        {
            foreach (MonoBehaviour script in scriptsToDisableOnDeath)
            {
                if (script != null && script.enabled)
                {
                    script.enabled = false;
                    Debug.Log($"Script desativado: {script.GetType().Name}");
                }
            }
        }

        enabled = false; // Desativa o DamageHandler

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; // Oculta o sprite principal
        }

        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        for (int i = 0; i < numberOfTomatoChunks; i++)
        {
            if (tomatoChunkPrefab != null)
            {
                GameObject tomatoChunk = Instantiate(tomatoChunkPrefab, transform.position, Random.rotation);
                Rigidbody2D rbChunk = tomatoChunk.GetComponent<Rigidbody2D>();
                if (rbChunk != null)
                {
                    Vector2 randomDirection = Random.insideUnitCircle.normalized;
                    rbChunk.AddForce(randomDirection * explosionForce, ForceMode2D.Impulse);
                }
                Destroy(tomatoChunk, deathEffectDuration);
            }
        }

        if (bloodParticlePrefab != null)
        {
            Instantiate(bloodParticlePrefab, transform.position, Quaternion.identity);
        }

        // Chama o evento de morte do jogador
        OnPlayerDeath?.Invoke();

        StartCoroutine(ShowGameOverScreen(deathEffectDuration));
    }

    IEnumerator ShowGameOverScreen(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
            Debug.Log("ShowGameOverScreen: gameOverPanelCanvasGroup não é nulo? " + (gameOverPanelCanvasGroup != null));
            if (gameOverPanelCanvasGroup != null)
            {
                Debug.Log("ShowGameOverScreen: Iniciando fade-in do gameOverPanelCanvasGroup.");
                float timer = 0f;
                gameOverPanelCanvasGroup.alpha = 0f;
                while (timer < fadeInDuration)
                {
                    timer += Time.deltaTime;
                    gameOverPanelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
                    Debug.Log("ShowGameOverScreen: Timer: " + timer + ", Alpha do Panel: " + gameOverPanelCanvasGroup.alpha);
                    yield return null;
                }
                gameOverPanelCanvasGroup.alpha = 1f;
                Debug.Log("ShowGameOverScreen: Fade-in do gameOverPanelCanvasGroup terminado.");
            }
            yield return StartCoroutine(TypewriterEffect(gameOverMessageText, GetRandomGameOverMessage()));
            // Após a digitação terminar, inicia o fade in do texto de reiniciar
            StartCoroutine(FadeInRestartText());
        }
    }

    IEnumerator TypewriterEffect(TMP_Text textComponent, string textToType)
    {
        textComponent.text = "";
        float t = 0;
        int charIndex = 0;
        isTyping = true; // Define a flag de digitação

        while (charIndex < textToType.Length)
        {
            t += Time.deltaTime * textRevealSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);
            textComponent.text = textToType.Substring(0, charIndex);
            yield return null;
        }
        isTyping = false; // Reseta a flag de digitação
    }

    string GetRandomGameOverMessage()
    {
        if (sarcasticGameOverMessages == null || sarcasticGameOverMessages.Length == 0)
        {
            Debug.LogWarning("Nenhuma mensagem de Game Over definida!");
            return "Game Over"; // Mensagem padrão
        }
        return sarcasticGameOverMessages[Random.Range(0, sarcasticGameOverMessages.Length)];
    }

    IEnumerator FadeInRestartText()
    {
        // Espera um pequeno atraso para garantir que a mensagem de Game Over esteja visível
        yield return new WaitForSeconds(restartTextDelay);

        if (restartTextCanvasGroup == null)
        {
            Debug.LogWarning("Canvas Group do texto de reiniciar não configurado!");
            yield break;
        }

        float timer = 0f;
        restartTextCanvasGroup.alpha = 0f;
        if (restartTextObject != null) restartTextObject.SetActive(true);

        while (timer < restartTextFadeInDuration)
        {
            timer += Time.deltaTime;
            restartTextCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / restartTextFadeInDuration);
            yield return null;
        }
        restartTextCanvasGroup.alpha = 1f;
    }

    void Update()
    {
        // Verifica o input primário para reiniciar a fase APENAS se a tela de Game Over estiver ativa
        if (gameOverCanvas != null && gameOverCanvas.activeSelf && !isTyping && Input.GetButtonDown("Fire1"))
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}