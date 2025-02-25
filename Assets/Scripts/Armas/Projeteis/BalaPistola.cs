public class BalaPistola : Projectile
{
    void Awake()
    {
        // Configurações para o projétil básico
        speed = 15f;         // Velocidade média
        damage = 5f;         // Dano pequeno
        lifetime = 2f;       // Alcance médio (tempo de vida)
        isBouncing = false;
        isBoomerang = false;
        isRocket = false;
    }
}