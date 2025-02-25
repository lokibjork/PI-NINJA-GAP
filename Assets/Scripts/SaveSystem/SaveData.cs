using System;

[Serializable]
public class SaveData
{
    public string playerName; // Nome do jogador
    public int playerLevel;   // Nível atual do jogador
    public float playTime;    // Tempo de jogo em horas
    public bool hasSave;      // Indica se o slot já tem um save válido

    public SaveData()
    {
        playerName = "Novo Jogador";
        playerLevel = 1;
        playTime = 0f;
        hasSave = false;
    }
}