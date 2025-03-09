using UnityEngine;

public class PizzaGun : Weapon
{
    private void Start()
    {
        weaponName = "Arma de Pizza";
        maxAmmo = 10;
        currentAmmo = maxAmmo;
        fireRate = 3f;      // Tempo alto de recarga
        // projectilePrefab (com script de boomerang) e shootPoint devem ser configurados no Inspector
    }

    public override void Shoot()
    {
        if (currentAmmo > 0 && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            
            // Ajusta a rotação conforme o flip do shootPoint
            Quaternion projRotation = shootPoint.rotation;
            if (shootPoint.lossyScale.x < 0)
            {
                projRotation = Quaternion.Euler(projRotation.eulerAngles + new Vector3(0, 0, 180));
            }
            
            Instantiate(projectilePrefab, shootPoint.position, projRotation);
            currentAmmo--;
            weaponManager.UpdateAmmoUI(currentAmmo, maxAmmo);
        }
        else
        {
            Debug.Log("Sem munição na Arma de Pizza!");
        }
    }
}
