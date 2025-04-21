using UnityEngine;

public class MuzzleFlashEffect : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Se usar um sprite
    public Animator animator;           // Se usar uma anima��o
    public float duration = 0.1f;       // Dura��o do efeito em segundos
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
            animator.Play("MuzzleFlash"); // Nome da sua anima��o
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            gameObject.SetActive(false); // Desativa o GameObject ap�s a dura��o
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
            animator.StopPlayback(); // Opcional: parar a anima��o ao desativar
        }
    }
}