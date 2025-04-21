using UnityEngine;


public class Pistola : Weapon
{
    [Header("Configurações da Pistola")]
    public AudioClip pistolClip;

    [Header("Configurações de Impulso (Ao Mirar para Baixo)")]
    public float impulsoParaCimaForca = 5f; // Força do impulso para cima ao atirar para baixo
    public PlayerWeaponControl playerWeaponControl; // Referência ao script de controle da arma do jogador
    public Rigidbody2D playerRb; // Referência ao Rigidbody2D do jogador
    public PlayerMovement playerMovement;
    public Animator anim; // Referência ao script de movimento do jogador (opcional)

    [Header("Ejeção de Cápsula")]
    public EjetarCapsula ejectorDeCapsula; // Referência ao script de ejeção de cápsula

    [Header("Recuo do Disparo")]
    public float recuoForca = 2f; // Força do recuo ao disparar

    private void Start()
    {
        weaponName = "Arma de Impulso";
        infiniteAmmo = true;
        fireRate = 4f;
        anim = GetComponent<Animator>();

        // Certifica-se de que as referências estão atribuídas
        if (playerWeaponControl == null)
        {
            Debug.LogError("PlayerWeaponControl não atribuído na " + weaponName);
            enabled = false;
        }
        if (playerRb == null)
        {
            Debug.LogError("Rigidbody2D do jogador não atribuído na " + weaponName);
            enabled = false;
        }
        if (playerMovement == null)
        {
            Debug.LogWarning("PlayerMovement não atribuído na " + weaponName + ". O impulso funcionará mesmo no ar.");
        }
        if (ejectorDeCapsula == null)
        {
            Debug.LogWarning("EjetarCapsula não atribuído na " + weaponName + ". A ejeção de cápsulas não funcionará.");
        }
    }

    public override void Shoot()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;

            // Ajusta a rotação do projétil considerando o flip
            Quaternion projRotation = shootPoint.rotation;
            Vector2 shootDirection = shootPoint.right; // Direção para a direita do ponto de disparo
            if (shootPoint.lossyScale.x < 0)
            {
                projRotation = Quaternion.Euler(projRotation.eulerAngles + new Vector3(0, 0, 180));
                shootDirection = -shootDirection; // Inverte a direção se a arma estiver flipada
            }

            Instantiate(projectilePrefab, shootPoint.position, projRotation);
            SoundManagerSO.PlaySoundFXClip(pistolClip, transform.position, 1f);
            anim.SetTrigger("isShooting");

            // Instancia o efeito de fumaça
            SpawnSmokeEffect();

            // Aplica o recuo ao jogador
            if (playerRb != null)
            {
                playerRb.AddForce(-shootDirection * recuoForca, ForceMode2D.Impulse);
            }

            // Ejeta a cápsula
            if (ejectorDeCapsula != null)
            {
                ejectorDeCapsula.Ejetar();
            }

            // Verifica se a arma está mirando para baixo e se o jogador não está no chão (opcional)
            if (playerWeaponControl.aimDirection == Vector2.down && (playerMovement == null || !playerMovement.isGrounded))
            {
                // Aplica uma força para cima no Rigidbody do jogador
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0f); // Reset na velocidade vertical antes de aplicar o impulso (opcional)
                playerRb.AddForce(Vector2.up * impulsoParaCimaForca, ForceMode2D.Impulse);
            }
        }
        else
        {
            anim.SetTrigger("isShooting");
        }
    }
}