using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public Weapon[] weapons; // Array com as armas
    public int currentWeaponIndex = 0;

    public InputActionAsset inputActions;
    private InputAction changeWeaponAction;
    private InputAction AttackAction;

    public float switchCooldown = 0.3f;
    private float lastSwitchTime = 0f;

    void Awake()
    {
        changeWeaponAction = inputActions.FindAction("ChangeWeapon");
        AttackAction = inputActions.FindAction("Attack");
    }

    void Start()
    {
        EquipWeapon(0);
    }

    void Update()
    {
        if (changeWeaponAction.ReadValue<float>() > 0 && Time.time - lastSwitchTime >= switchCooldown)
        {
            SwitchWeapon();
            lastSwitchTime = Time.time;
        }
    }

    public void EquipWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            bool isActive = (i == index);
            weapons[i].gameObject.SetActive(isActive);
            Debug.Log($"Arma {weapons[i].name} ativa? {isActive}");
        }
    }

    public void SwitchWeapon()
    {
        int newIndex = (currentWeaponIndex + 1) % weapons.Length;
        while (!weapons[newIndex].isUnlocked)
        {
            newIndex = (newIndex + 1) % weapons.Length;
        }
        currentWeaponIndex = newIndex;
        EquipWeapon(currentWeaponIndex);
    }

    public void UnlockWeapon(int index)
    {
        weapons[index].isUnlocked = true;
    }
}