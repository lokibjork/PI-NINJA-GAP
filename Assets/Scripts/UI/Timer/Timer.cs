using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [Header("Configurações do Timer")]
    public float initialTime = 60f; // Tempo inicial em segundos
    public float warningTime = 10f; // Tempo restante para começar a avisar
    public Color warningColor = Color.red; // Cor do texto quando o tempo está acabando
    public float warningPulseSpeed = 1f; // Velocidade do pulso da cor de aviso
    public string timeFormat = "00"; // Formato de exibição do tempo (ex: "00", "00.0", "0:00")

    [Header("Referências da UI")]
    public TMP_Text timerText; // Texto da UI para exibir o tempo

    [Header("Controle de Inicialização")]
    public Intro introScript; // Arraste o GameObject com o script Intro aqui
    private bool timerStarted = false;

    private float currentTime;
    private Color originalColor;
    private bool isWarning = false;

    void Start()
    {
        currentTime = initialTime;
        if (timerText != null)
        {
            originalColor = timerText.color;
            UpdateTimerDisplay();
        }
        else
        {
            Debug.LogError("Texto do Timer não atribuído! Arraste um TextMeshProUGUI para a variável 'Timer Text' no Inspector.");
            enabled = false; // Desativa o script se não houver texto para exibir
            return;
        }

        // Se o script Intro estiver referenciado, espera pelo evento de finalização
        if (introScript != null)
        {
            introScript.onIntroSequenceEnded.AddListener(StartTimerAfterIntro);
            enabled = false; // Desativa o timer no início
        }
        else
        {
            // Se não houver Intro, inicia o timer imediatamente (comportamento padrão)
            StartTimer();
        }
    }

    void Update()
    {
        if (timerStarted)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                UpdateTimerDisplay();

                if (currentTime <= warningTime && !isWarning)
                {
                    isWarning = true;
                    // Adicione aqui qualquer outra ação de aviso (som, animação, etc.)
                }
            }
            else
            {
                currentTime = 0;
                UpdateTimerDisplay();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                enabled = false; // Desativa o timer quando o tempo acaba
            }

            // Efeito de pulso da cor de aviso
            if (isWarning && timerText != null)
            {
                float pulse = Mathf.Sin(Time.time * warningPulseSpeed) * 0.5f + 0.5f; // Varia entre 0 e 1
                timerText.color = Color.Lerp(originalColor, warningColor, pulse);
            }
            else if (timerText != null)
            {
                timerText.color = originalColor; // Garante que a cor volte ao normal se o aviso acabar
            }
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            // Formata o tempo de acordo com o formato especificado
            string formattedTime = FormatTime(currentTime);
            timerText.text = formattedTime;
        }
    }

    string FormatTime(float time)
    {
        switch (timeFormat)
        {
            case "00":
                return Mathf.FloorToInt(time).ToString("00");
            case "00.0":
                return time.ToString("F1"); // Uma casa decimal
            case "0:00":
                int minutes = Mathf.FloorToInt(time / 60);
                int seconds = Mathf.FloorToInt(time % 60);
                return string.Format("{0}:{1:00}", minutes, seconds);
            default:
                return Mathf.FloorToInt(time).ToString();
        }
    }

    // Método para iniciar o timer externamente
    public void StartTimer()
    {
        enabled = true;
        timerStarted = true;
        currentTime = initialTime;
        isWarning = false;
        if (timerText != null)
        {
            timerText.color = originalColor;
        }
    }

    // Método para ser chamado pelo evento de finalização da Intro
    public void StartTimerAfterIntro()
    {
        StartTimer();
    }

    // Método para parar o timer externamente (opcional)
    public void StopTimer()
    {
        enabled = false;
        timerStarted = false;
    }

    // Método para adicionar tempo ao timer (power-ups, etc.)
    public void AddTime(float amount)
    {
        currentTime += amount;
        if (currentTime <= warningTime)
        {
            isWarning = true;
        }
    }

    // Método para subtrair tempo do timer (penalidades, etc.)
    public void SubtractTime(float amount)
    {
        currentTime -= amount;
        if (currentTime <= warningTime && currentTime > 0)
        {
            isWarning = true;
        }
        else if (currentTime <= 0)
        {
            currentTime = 0;
            UpdateTimerDisplay();
            enabled = false;
            timerStarted = false;
        }
    }
}