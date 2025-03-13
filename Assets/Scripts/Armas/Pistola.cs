using UnityEngine;


public class Pistola : Weapon
{
    public AudioClip pistolClip;
    private void Start()
    {
        weaponName = "Arma Básica";
        // Munição infinita pode ser tratada ignorando o decremento (ou usando uma flag)
        infiniteAmmo = true;
        fireRate = 4f;
        // Certifique-se de definir projectilePrefab e shootPoint via Inspector
    }

    public override void Shoot()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            
            // Ajusta a rotação considerando o flip
            Quaternion projRotation = shootPoint.rotation;
            if (shootPoint.lossyScale.x < 0)
            {
                projRotation = Quaternion.Euler(projRotation.eulerAngles + new Vector3(0, 0, 180));
            }
            
            Instantiate(projectilePrefab, shootPoint.position, projRotation);
            SoundManagerSO.PlaySoundFXClip(pistolClip, transform.position, 1f);
        }
    }
}