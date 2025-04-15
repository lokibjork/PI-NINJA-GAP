using UnityEngine;

public class SegundaFase : MonoBehaviour
{
    public GameObject player;
    public Vector3 novaPosicao = new Vector3(0f, 0f, 0f); // Coloca aqui a posi��o que tu quer

    void Start()
    {
        if (player != null)
        {
            player.transform.position = novaPosicao;
        }
        else
        {
            Debug.LogWarning("Player n�o foi atribu�do no Inspector!");
        }
    }

    private void Awake()
    {
        if (player != null)
        {
            player.transform.position = novaPosicao;
        }
        else
        {
            Debug.LogWarning("Player n�o foi atribu�do no Inspector!");
        }
    }
}
