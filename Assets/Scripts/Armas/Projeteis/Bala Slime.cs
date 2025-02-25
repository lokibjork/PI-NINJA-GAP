using UnityEngine;
    public class BalaSlime : Projectile
    {
        void Awake()
        {
            // Configurações para o projétil de slime
            speed = 10f;         // Velocidade moderada
            damage = 5f;         // Dano baixo
            lifetime = 4f;       // Tempo suficiente para quicar
            isBouncing = true;   // Ativa o comportamento de quique
            isBoomerang = false;
            isRocket = false;
        }
    }