using UnityEngine;
using UnityEngine.SceneManagement;

public class GerenciadorDeCena : MonoBehaviour
{
    public static GerenciadorDeCena Instance { get; private set; }

    [Header("Configuração de Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private string playerTag = "Player";
    private GameObject currentPlayerInstance;

    [Header("Posição Padrão da Cena")]
    [SerializeField] private Vector3 defaultSpawnPosition = new Vector3(0f, 0f, 0f);

    [Header("Configurações de Debug")]
    public bool forceDefaultSpawn = false;
    public bool clearPlayerPrefsOnStart = false; // Isso limpará TUDO no PlayerPrefs, incluindo configurações de jogo
    public bool clearCheckpointOnStart = false; // NOVO: Limpa APENAS o checkpoint no início do jogo

    private string lastLoadedSceneName = ""; // NOVO: Para rastrear a última cena carregada

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
            Debug.LogWarning("PlayerPrefs LIMPOS no início do jogo (DEBUG ON).");
        }
        else if (clearCheckpointOnStart) // Só executa se clearPlayerPrefsOnStart não estiver ativo
        {
            ClearCheckpointData();
            Debug.LogWarning("Dados de Checkpoint LIMPOS no início do jogo (DEBUG ON).");
        }

        SceneManager.sceneLoaded += OnSceneLoaded; // Assina o evento para quando uma cena é carregada

        // Inicializa o nome da última cena carregada para a cena atual
        lastLoadedSceneName = SceneManager.GetActiveScene().name;

        // Posiciona o player na cena atual se este script já estava ativo
        PositionPlayerInCurrentScene();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Chamado sempre que uma nova cena é carregada
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GerenciadorDeCena: Cena '{scene.name}' carregada. Modo: {mode}.");

        // NOVO: Verifica se a cena carregada é diferente da anterior para resetar o checkpoint
        if (lastLoadedSceneName != "" && scene.name != lastLoadedSceneName)
        {
            Debug.Log($"Transição de cena detectada ({lastLoadedSceneName} -> {scene.name}). Limpando dados de checkpoint.");
            ClearCheckpointData();
        }

        lastLoadedSceneName = scene.name; // Atualiza o nome da última cena carregada

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
                    Debug.LogError("Player Prefab não atribuído e nenhum Player encontrado com a tag '" + playerTag + "'. Não foi possível posicionar o player.");
                    return;
                }
            }
        }

        Vector3 finalSpawnPosition = defaultSpawnPosition;
        string spawnReason = "posição padrão da fase";

        // Verifica se o checkpoint deve ser ignorado ou se não existe
        if (!forceDefaultSpawn && PlayerPrefs.HasKey("CheckpointX") && PlayerPrefs.HasKey("CheckpointY"))
        {
            float checkpointX = PlayerPrefs.GetFloat("CheckpointX");
            float checkpointY = PlayerPrefs.GetFloat("CheckpointY");
            finalSpawnPosition = new Vector3(checkpointX, checkpointY, currentPlayerInstance.transform.position.z);
            spawnReason = "posição do checkpoint";
        }

        currentPlayerInstance.transform.position = finalSpawnPosition;
        Debug.Log($"Player posicionado em {finalSpawnPosition} (motivo: {spawnReason}).");
    }

    // NOVO: Método para limpar apenas os dados do checkpoint
    public void ClearCheckpointData()
    {
        PlayerPrefs.DeleteKey("CheckpointX");
        PlayerPrefs.DeleteKey("CheckpointY");
        PlayerPrefs.DeleteKey("CheckpointID");
        PlayerPrefs.Save(); // Garante que as mudanças sejam salvas no disco
    }

    // Método para ser chamado por outros scripts (ex: quando o player morre e precisa recarregar)
    public void ReloadCurrentScene()
    {
        currentPlayerInstance = null; // Zera a instância para que seja encontrada novamente
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Método para carregar uma nova cena e resetar o checkpoint
    public void LoadNewScene(string sceneName)
    {
        // Ao carregar uma nova cena, o evento OnSceneLoaded vai detectar a mudança
        // e limpar o checkpoint automaticamente.
        currentPlayerInstance = null; // Zera a instância para que seja encontrada novamente
        SceneManager.LoadScene(sceneName);
    }
}