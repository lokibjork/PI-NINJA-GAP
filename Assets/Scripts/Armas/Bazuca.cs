using UnityEngine;

public class Bazuca : Weapon
{
    private void Start()
    {
        weaponName = "Bazuca";
        maxAmmo = 5;
        currentAmmo = maxAmmo;
        fireRate = 0.5f; // disparo mais lento

        // Defina aqui o projectilePrefab e shootPoint específicos da bazuca
    }

    public override void Shoot()
    {
        if (currentAmmo > 0 && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
        
            // Pega a rotação atual do shootPoint
            Quaternion projRotation = shootPoint.rotation;
            // Se o shootPoint estiver flipado horizontalmente (scale.x negativo), inverta a rotação em Z
            if (shootPoint.lossyScale.x < 0)
            {
                projRotation = Quaternion.Euler(projRotation.eulerAngles + new Vector3(0, 0, 180));
            }
        
            Instantiate(projectilePrefab, shootPoint.position, projRotation);
            currentAmmo--;
            Debug.Log("Bazuca atirou!");
        }
        else
        {
            Debug.Log("Sem munição na Bazuca!");
        }
    }
}