using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    // Propriedades da arma
    public Sprite weaponIcon;
    public string weaponName = "";
    public int maxAmmo;             // Munição máxima
    public int currentAmmo; // Munição atual
    public bool infiniteAmmo = false;
    public WeaponManager weaponManager;

    public float fireRate;          // Taxa de disparo

    public bool isUnlocked = false; // Arma desbloqueada?

    public GameObject projectilePrefab; // Prefab do projétil
    public Transform shootPoint;        // Ponto de disparo

    protected float nextFireTime;       // Controle de cooldown do disparo

    // Cada arma vai implementar seu próprio Shoot()
    public abstract void Shoot();
}