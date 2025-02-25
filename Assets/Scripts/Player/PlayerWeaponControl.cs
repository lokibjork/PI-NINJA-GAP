using UnityEngine;

public class PlayerWeaponControl : MonoBehaviour
{
    public Transform WeaponTransform;
    private Vector2 aimDirection = Vector2.right;
    public PlayerMovement _playerMovement;

    void Update()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        if (yInput > 0)
            aimDirection = Vector2.up;
        else if (yInput < 0)
            aimDirection = Vector2.down;
        else if (xInput > 0)
            aimDirection = Vector2.right;
        else if (xInput < 0)
            aimDirection = Vector2.left;

        // Se o jogador soltar o botão de mirar para cima, volta para direita/esquerda
        if (yInput == 0 && aimDirection == Vector2.up)
            aimDirection = _playerMovement.isFacingRight ? Vector2.right : Vector2.left;

        UpdateWeaponTransform(aimDirection);
    }

    void UpdateWeaponTransform(Vector2 direction)
    {
        if (direction == Vector2.up)
        {
            WeaponTransform.localPosition = new Vector3(0, 1, 0);
            WeaponTransform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (direction == Vector2.down && !_playerMovement.isGrounded)
        {
            WeaponTransform.localPosition = new Vector3(0, -1, 0);
            WeaponTransform.localRotation = Quaternion.Euler(0, 0, -90);
        }
        else if (direction == Vector2.right)
        {
            WeaponTransform.localPosition = new Vector3(1, 0, 0);
            WeaponTransform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction == Vector2.left) 
        {
            WeaponTransform.localPosition = new Vector3(1, 0, 0);
            WeaponTransform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        if (_playerMovement.isGrounded && direction != Vector2.up)
        {
            if (_playerMovement.isFacingRight)
            {
                WeaponTransform.localPosition = new Vector3(1, 0, 0);
                WeaponTransform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                WeaponTransform.localPosition = new Vector3(1, 0, 0);
                WeaponTransform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}
