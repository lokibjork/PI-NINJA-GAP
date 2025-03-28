using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private GameObject bossMinerador_;
    public BossMinerador bossMinerador;
    public Slider slider;

    private void Start()
    {
        if (bossMinerador_ != null)
        {
            bossMinerador = bossMinerador_.GetComponent<BossMinerador>();
        }
        else
        {
            Debug.LogError("bossMinerador_ n�o foi atribu�do na UI de vida do Boss!");
        }
    }

    private void Update()
    {
        if (bossMinerador != null)
        {
            slider.maxValue = bossMinerador.GetComponent<EnemyBase>().maxHealth;
            slider.value = bossMinerador.GetComponent<EnemyBase>().currentHealth;
        }
    }
}
