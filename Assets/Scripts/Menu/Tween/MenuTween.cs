using System;

using UnityEngine;

using UnityEngine.SceneManagement;



public class MenuTween : MonoBehaviour

{

    [SerializeField] private GameObject NewGameButton, OptionsButton, QuitButton, Logo;

    [SerializeField] private GameObject SettingsPanel, SettingsGamePadButton, SettingsKeyboardButton, SettingsBackButton;



    // Métodos para On Click() configurados via Unity Inspector



    private void OnEnable()

    {

        MainMenuIn();

        SettingsIn();

    }



    private void OnDisable()

    {

        MainMenuOut();

        SettingsOut();

    }



    public void MainMenuIn()
    {
        LeanTween.cancel(Logo);
        LeanTween.cancel(OptionsButton);
        LeanTween.cancel(NewGameButton);
        LeanTween.cancel(QuitButton);

        AnimateMenu(Logo, new Vector3(42, 250, 0), 2f);

        AnimateMenu(OptionsButton, new Vector3(-19, -271, 0), 1.5f);

        AnimateMenu(NewGameButton, new Vector3(0, -116, 0), 0.5f);

        AnimateMenu(QuitButton, new Vector3(0, -418, 0), 2f);

    }



    public void MainMenuOut()

    {
        AnimateMenu(Logo, new Vector3(42, 860, 0), 2f);

        AnimateMenu(NewGameButton, new Vector3(-1500, -121, 0), 0.5f);

        AnimateMenu(OptionsButton, new Vector3(-1500, -281, 0), 1.5f);

        AnimateMenu(QuitButton, new Vector3(-1500, -361, 0), 2f);

    }



    public void SettingsIn()

    {

        AnimateMenu(SettingsPanel, Vector3.zero, 3f);

        AnimateMenu(SettingsGamePadButton, new Vector3(0, 80, 0), 2f);

        AnimateMenu(SettingsKeyboardButton, Vector3.zero, 1.5f);

        AnimateMenu(SettingsBackButton, new Vector3(0, -80, 0), 1f);

    }



    public void SettingsOut()

    {

        AnimateMenu(SettingsPanel, new Vector3(0, -1500, 0), 3f);

        AnimateMenu(SettingsGamePadButton, new Vector3(0, -1500, 0), 2f);

        AnimateMenu(SettingsKeyboardButton, new Vector3(0, -1500, 0), 1.5f);

        AnimateMenu(SettingsBackButton, new Vector3(0, -1500, 0), 1f);

    }



    public void AnimateMenu(GameObject obj, Vector3 position, float time)

    {

        if (obj)

            LeanTween.moveLocal(obj, position, time).setEase(LeanTweenType.easeOutElastic);

    }



    public void NewGame()

    {
        SceneManager.LoadScene("Fase 1");
    }

}