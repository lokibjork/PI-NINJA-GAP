using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuCanvas;
    [SerializeField] private GameObject _settingsCanvas;
    
    [Header("Desativar features para pausar")]
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerShooting _playerShooting;
    [SerializeField] private PlayerWeaponControl _playerWeaponControl;
    
    [Header("Primeira seleção ao entrar no menu")]
    [SerializeField] private GameObject _mainMenuFirst;
    [SerializeField] private GameObject _settingsMenuFirst;

    private bool isPaused;

    private void Start()
    {
        _mainMenuCanvas.SetActive(false);
        _settingsCanvas.SetActive(false);
    }

    private void Update()
    {
        if (InputManager.instance.MenuOpenCloseInput)
        {
            if (!isPaused)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }
    }

    // Pause/Unpause Functions
    private void Pause()
    {
        isPaused = true;
        Time.timeScale = 0;
        
        _playerMovement.enabled = false;
        _playerShooting.enabled = false;
        _playerWeaponControl.enabled = false;

        OpenMainMenu();
    }
    
    private void Unpause()
    {
        isPaused = false;
        Time.timeScale = 1;
        
        _playerMovement.enabled = true;
        _playerShooting.enabled = true;
        _playerWeaponControl.enabled = true;

        CloseAllMenus();
    }


    

    #region Canvas Activations/Deactivations

    // ReSharper disable Unity.PerformanceAnalysis
    private void OpenMainMenu()
    {
        _mainMenuCanvas.SetActive(true);
        _settingsCanvas.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
    }

    private void OpenSettingsMenuHandle()
    {
        _settingsCanvas.SetActive(true);
        _mainMenuCanvas.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_settingsMenuFirst);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void CloseAllMenus()
    {
        _mainMenuCanvas.SetActive(false);
        _settingsCanvas.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(null);
    }

    #endregion

    #region Main Menu Button Actions

    public void OnSettingsPress()
    {
        OpenSettingsMenuHandle();
    }
    
    public void OnResumePress()
    {
        Unpause();
    }
    #endregion

    #region Settings Menu Button Actions

    public void OnSettingsBackPress()
    {
        OpenMainMenu();
    }

    #endregion

    public void BackMenu()
    {
        Time.timeScale = 1;
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Menu");
    }
}
