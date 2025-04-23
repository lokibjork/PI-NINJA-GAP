using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public BossHealth bossHealth; // arrasta o boss aqui
    public Slider healthSlider;
    public Image fillImage;

    void Start()
    {
        healthSlider.maxValue = bossHealth.maxHealth;
        healthSlider.value = bossHealth.currentHealth;
    }

    void Update()
    {
        healthSlider.value = bossHealth.currentHealth;
    }
}
