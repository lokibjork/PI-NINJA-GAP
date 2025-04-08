using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    // Referência para o WeaponManager (que já cuida da troca de armas)
    public WeaponManager weaponManager;

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            // Pegamos a arma ativa pelo índice que o WeaponManager mantém
            Weapon currentWeapon = weaponManager.weapons[weaponManager.currentWeaponIndex];
            if (currentWeapon != null)
            {
                currentWeapon.Shoot();
            }
        }
    }
}