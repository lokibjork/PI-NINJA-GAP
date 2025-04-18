using UnityEngine;

public class SlimePistol : Weapon
{
    private void Start()
    {
        weaponName = "Pistola de Slime";
        maxAmmo = 15;
        currentAmmo = maxAmmo;
        fireRate = 5f;      // Tempo baixo de recarga
        // projectilePrefab e shootPoint devem ser configurados no Inspector
    }

    public override void Shoot()
    {
        if (currentAmmo > 0 && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            
            // Ajusta a rotação se o shootPoint estiver flipado
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
            Debug.Log("Sem munição na Pistola de Slime!");
        }
    }
}
