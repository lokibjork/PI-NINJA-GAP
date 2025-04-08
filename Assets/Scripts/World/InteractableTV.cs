using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class InteractableTV : MonoBehaviour
{
    public GameObject interactionIcon;
    public GameObject tvScreenUI;
    public Image tvImage;
    public TMP_Text tvText;

    public Sprite[] scenes;
    [TextArea]
    public string[] sceneTexts;

    private bool playerNear = false;
    private bool isWatching = false;
    private int currentScene = 0;
    public PlayerMovement playerMovement;
    public PlayerShooting playerShooting;
    public PlayerWeaponControl playerWeaponControl;

    [Header("Typing Effect")]
    [SerializeField] private float typingSpeed = 0.02f;
    private Coroutine typingCoroutine;

    void Start()
    {
        interactionIcon.SetActive(false);
        tvScreenUI.SetActive(false);
    }

    void Update()
    {
        if (playerNear && !isWatching && Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartWatching();
        }

        if (isWatching && Input.GetButtonDown("Fire1"))
        {
            NextScene();
        }
    }

    void StartWatching()
    {
        isWatching = true;
        currentScene = 0;
        tvScreenUI.SetActive(true);
        ShowScene();
        Time.timeScale = 0f; // Pausa o jogo
        playerMovement.enabled = false;
        playerShooting.enabled = false;
        playerWeaponControl.enabled = false;
    }

    void NextScene()
    {
        currentScene++;
        if (currentScene >= scenes.Length)
        {
            StopWatching();
        }
        else
        {
            ShowScene();
        }
    }

    void ShowScene()
    {
        tvImage.sprite = scenes[currentScene];

        // Efeito de texto digitando
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(sceneTexts[currentScene]));
    }

    IEnumerator TypeText(string message)
    {
        tvText.text = "";
        foreach (char c in message)
        {
            tvText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    void StopWatching()
    {
        isWatching = false;
        tvScreenUI.SetActive(false);
        Time.timeScale = 1f;
        playerMovement.enabled = true;
        playerShooting.enabled = true;
        playerWeaponControl.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            interactionIcon.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            interactionIcon.SetActive(false);
        }
    }
}
