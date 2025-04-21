using UnityEngine;

public class MuzzleFlashEffect : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Se usar um sprite
    public Animator animator;           // Se usar uma animação
    public float duration = 0.1f;       // Duração do efeito em segundos
    private float timer;

    void OnEnable()
    {
        timer = duration;
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        if (animator != null)
        {
            animator.Play("MuzzleFlash"); // Nome da sua animação
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            gameObject.SetActive(false); // Desativa o GameObject após a duração
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }
        }
    }

    void OnDisable()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        if (animator != null)
        {
            animator.StopPlayback(); // Opcional: parar a animação ao desativar
        }
    }
}