using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    private MusicManager musicaManager;

    private string MusicaLevel11 = "Level 1 Cave";


    private void Awake()
    {
        // Implementação do Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        MusicManager.Instance.PlayTrack(MusicaLevel11);
    }
}
