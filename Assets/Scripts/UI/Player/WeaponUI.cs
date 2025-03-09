using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public Image weaponIcon;
    public TMP_Text ammoText;

    public void UpdateWeaponUI(Sprite newIcon, int currentAmmo, int maxAmmo)
    {
        weaponIcon.sprite = newIcon;
        ammoText.text = $"{currentAmmo} / {maxAmmo}";
    }

    public void UpdateAmmoUI(int currentAmmo, int maxAmmo)
    {
        ammoText.text = $"{currentAmmo} / {maxAmmo}";
    }
}