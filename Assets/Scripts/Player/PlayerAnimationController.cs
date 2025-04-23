using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;
    private PlayerWeaponControl weaponControl;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        weaponControl = GetComponent<PlayerWeaponControl>();

        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement script n�o encontrado!");
        }
        if (weaponControl == null)
        {
            Debug.LogError("PlayerWeaponControl script n�o encontrado!");
        }
    }

    void Update()
    {
        if (playerMovement == null || weaponControl == null || animator == null)
        {
            return; // N�o continue se as refer�ncias n�o forem encontradas
        }

        // 1. Controlar o par�metro VerticalAim baseado na dire��o da mira
        int verticalAimDirection = 0;
        if (weaponControl.aimDirection == Vector2.up)
        {
            verticalAimDirection = 1;
        }
        else if (weaponControl.aimDirection == Vector2.down)
        {
            {
                verticalAimDirection = -1;
            }
            animator.SetInteger("VerticalAim", verticalAimDirection);

            // 2. Controlar o par�metro IsJumping baseado no estado de pulo
            animator.SetBool("IsJumping", !playerMovement.isGrounded);

            // 3. Controlar o par�metro IsMoving baseado no movimento horizontal
            animator.SetBool("IsMoving", Mathf.Abs(playerMovement.rb.linearVelocity.x) > 0.01f);
        }
    }
}