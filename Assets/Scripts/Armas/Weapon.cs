using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    // Propriedades da arma
    public Sprite weaponIcon;
    public string weaponName = "";
    public int maxAmmo;                 // Munição máxima
    public int currentAmmo; // Munição atual
    public bool infiniteAmmo = false;
    public WeaponManager weaponManager;

    public float fireRate;              // Taxa de disparo

    public bool isUnlocked = false; // Arma desbloqueada?

    public GameObject projectilePrefab; // Prefab do projétil
    public Transform shootPoint;
    public Transform shellPoint;// Ponto de disparo
    protected float nextFireTime;        // Controle de cooldown do disparo

    [Header("Efeito de Fumaça")]
    public GameObject smokeEffectPrefab; // Prefab do efeito de fumaça
    public Transform smokePoint; // Ponto de emissão da fumaça

    // Cada arma vai implementar seu próprio Shoot()
    public abstract void Shoot();

    // Método comum para instanciar o efeito de fumaça
    protected void SpawnSmokeEffect()
    {
        if (smokeEffectPrefab != null && smokePoint != null)
        {
            GameObject smoke = Instantiate(smokeEffectPrefab, smokePoint.position, smokePoint.rotation);
            Destroy(smoke, 3f); // Ajuste o tempo conforme a duração da fumaça
        }
    }
}