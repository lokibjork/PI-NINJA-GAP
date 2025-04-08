using UnityEngine;
using UnityEngine.EventSystems;

public class AnimatedButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public AudioSource hoverSound;
    private Vector3 originalScale;
    private LTDescr activeTween;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (hoverSound != null) hoverSound.Play();
        
        activeTween = LeanTween.scale(gameObject, originalScale * 1.1f, 0.5f)
            .setEase(LeanTweenType.easeInOutSine)
            .setLoopPingPong();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        // Cancelar a animação e restaurar o tamanho original
        if (activeTween != null)
        {
            LeanTween.cancel(gameObject);
            activeTween = null;
        }
        transform.localScale = originalScale;
    }
}
