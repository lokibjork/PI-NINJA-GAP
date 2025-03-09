using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public Image weaponIconImage;  // Referência à imagem na UI
    public TMP_Text ammoText;         // Referência ao texto de munição

    public void UpdateWeaponUI(Sprite icon, int currentAmmo, int maxAmmo)
    {
        if (weaponIconImage != null && icon != null)
        {
            weaponIconImage.sprite = icon;
        }
        UpdateAmmoUI(currentAmmo, maxAmmo);
    }

    public void UpdateAmmoUI(int currentAmmo, int maxAmmo)
    {
        ammoText.text = $"{currentAmmo} / {maxAmmo}";
    }
}