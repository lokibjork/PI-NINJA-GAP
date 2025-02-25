using UnityEngine;
using UnityEngine.UI;

public class WeaponShop : MonoBehaviour
{
    public WeaponManager weaponManager;
    public Player.PlayerData playerData;  // Agora usa o PlayerData atualizado
    public int[] weaponPrices;            // Preço de cada arma
    public Button[] buyButtons;           // Botões da loja para cada arma

    void Start()
    {
        UpdateShopUI();
    }

    public void BuyWeapon(int index)
    {
        if (playerData.dinheiro >= weaponPrices[index])
        {
            playerData.dinheiro -= weaponPrices[index]; // Deduz o dinheiro
            weaponManager.UnlockWeapon(index);          // Desbloqueia a arma no WeaponManager
            UpdateShopUI();                             // Atualiza a interface da loja
        }
        else
        {
            Debug.Log("Moedas insuficientes!");
        }
    }

    private void UpdateShopUI()
    {
        // Habilita ou desabilita os botões de compra conforme se a arma já está desbloqueada
        for (int i = 0; i < buyButtons.Length; i++)
        {
            buyButtons[i].interactable = !weaponManager.weapons[i].isUnlocked;
        }
    }
}