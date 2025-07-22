using UnityEngine;
using UnityEngine.SceneManagement;

public class GerenciadorDeCena : MonoBehaviour
{
    public static GerenciadorDeCena Instance { get; private set; }

    [Header("Configura��o de Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private string playerTag = "Player";
    private GameObject currentPlayerInstance;

    [Header("Posi��o Padr�o da Cena")]
    [SerializeField] private Vector3 defaultSpawnPosition = new Vector3(0f, 0f, 0f);

    [Header("Configura��es de Debug")]
    public bool forceDefaultSpawn = false;
    public bool clearPlayerPrefsOnStart = false; // Isso limpar� TUDO no PlayerPrefs, incluindo configura��es de jogo
    public bool clearCheckpointOnStart = false; // NOVO: Limpa APENAS o checkpoint no in�cio do jogo

    private string lastLoadedSceneName = ""; // NOVO: Para rastrear a �ltima cena carregada

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (clearPlayerPrefsOnStart)
        {
            PlayerPrefs.DeleteAll();
            Debug.LogWarning("PlayerPrefs LIMPOS no in�cio do jogo (DEBUG ON).");
        }
        else if (clearCheckpointOnStart) // S� executa se clearPlayerPrefsOnStart n�o estiver ativo
        {
            ClearCheckpointData();
            Debug.LogWarning("Dados de Checkpoint LIMPOS no in�cio do jogo (DEBUG ON).");
        }

        SceneManager.sceneLoaded += OnSceneLoaded; // Assina o evento para quando uma cena � carregada

        // Inicializa o nome da �ltima cena carregada para a cena atual
        lastLoadedSceneName = SceneManager.GetActiveScene().name;

        // Posiciona o player na cena atual se este script j� estava ativo
        PositionPlayerInCurrentScene();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Chamado sempre que uma nova cena � carregada
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GerenciadorDeCena: Cena '{scene.name}' carregada. Modo: {mode}.");

        // NOVO: Verifica se a cena carregada � diferente da anterior para resetar o checkpoint
        if (lastLoadedSceneName != "" && scene.name != lastLoadedSceneName)
        {
            Debug.Log($"Transi��o de cena detectada ({lastLoadedSceneName} -> {scene.name}). Limpando dados de checkpoint.");
            ClearCheckpointData();
        }

        lastLoadedSceneName = scene.name; // Atualiza o nome da �ltima cena carregada

        PositionPlayerInCurrentScene();
    }

    private void PositionPlayerInCurrentScene()
    {
        if (currentPlayerInstance == null)
        {
            currentPlayerInstance = GameObject.FindGameObjectWithTag(playerTag);
            if (currentPlayerInstance == null)
            {
                if (playerPrefab != null)
                {
                    currentPlayerInstance = Instantiate(playerPrefab);
                    currentPlayerInstance.tag = playerTag;
                    Debug.Log("Player prefab instanciado na cena.");
                }
                else
                {
                    Debug.LogError("Player Prefab n�o atribu�do e nenhum Player encontrado com a tag '" + playerTag + "'. N�o foi poss�vel posicionar o player.");
                    return;
                }
            }
        }

        Vector3 finalSpawnPosition = defaultSpawnPosition;
        string spawnReason = "posi��o padr�o da fase";

        // Verifica se o checkpoint deve ser ignorado ou se n�o existe
        if (!forceDefaultSpawn && PlayerPrefs.HasKey("CheckpointX") && PlayerPrefs.HasKey("CheckpointY"))
        {
            float checkpointX = PlayerPrefs.GetFloat("CheckpointX");
            float checkpointY = PlayerPrefs.GetFloat("CheckpointY");
            finalSpawnPosition = new Vector3(checkpointX, checkpointY, currentPlayerInstance.transform.position.z);
            spawnReason = "posi��o do checkpoint";
        }

        currentPlayerInstance.transform.position = finalSpawnPosition;
        Debug.Log($"Player posicionado em {finalSpawnPosition} (motivo: {spawnReason}).");
    }

    // NOVO: M�todo para limpar apenas os dados do checkpoint
    public void ClearCheckpointData()
    {
        PlayerPrefs.DeleteKey("CheckpointX");
        PlayerPrefs.DeleteKey("CheckpointY");
        PlayerPrefs.DeleteKey("CheckpointID");
        PlayerPrefs.Save(); // Garante que as mudan�as sejam salvas no disco
    }

    // M�todo para ser chamado por outros scripts (ex: quando o player morre e precisa recarregar)
    public void ReloadCurrentScene()
    {
        currentPlayerInstance = null; // Zera a inst�ncia para que seja encontrada novamente
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // M�todo para carregar uma nova cena e resetar o checkpoint
    public void LoadNewScene(string sceneName)
    {
        // Ao carregar uma nova cena, o evento OnSceneLoaded vai detectar a mudan�a
        // e limpar o checkpoint automaticamente.
        currentPlayerInstance = null; // Zera a inst�ncia para que seja encontrada novamente
        SceneManager.LoadScene(sceneName);
    }
}