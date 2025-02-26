using UnityEngine;

public class BalaBazuca : Projectile
{
    void Awake()
    {
        // Configurações para o projétil da bazuca
        speed = 15f;          // Lento
        damage = 4;        // Dano alto
        lifetime = 1f;       // Pode ter um tempo de vida maior para simular a trajetória lenta
        isRocket = true;     // Ativa comportamento de foguete (explosão)
        isBouncing = false;
        isBoomerang = false;
    }
}
