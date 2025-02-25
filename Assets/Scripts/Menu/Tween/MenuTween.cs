using System;
using UnityEngine;

public class MenuTween : MonoBehaviour
{
    [SerializeField] private GameObject NewGameButton, ContinueButton, OptionsButton, QuitButton;
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

    private void MainMenuIn()
    {
        AnimateMenu(NewGameButton, new Vector3(-696, -121, 0), 0.5f);
        AnimateMenu(ContinueButton, new Vector3(-696, -201, 0), 1f);
        AnimateMenu(OptionsButton, new Vector3(-696, -281, 0), 1.5f);
        AnimateMenu(QuitButton, new Vector3(-696, -361, 0), 2f);
    }

    private void MainMenuOut()
    {
        AnimateMenu(NewGameButton, new Vector3(-1500, -121, 0), 0.5f);
        AnimateMenu(ContinueButton, new Vector3(-1500, -201, 0), 1f);
        AnimateMenu(OptionsButton, new Vector3(-1500, -281, 0), 1.5f);
        AnimateMenu(QuitButton, new Vector3(-1500, -361, 0), 2f);
    }

    private void SettingsIn()
    {
        AnimateMenu(SettingsPanel, Vector3.zero, 3f);
        AnimateMenu(SettingsGamePadButton, new Vector3(0, 80, 0), 2f);
        AnimateMenu(SettingsKeyboardButton, Vector3.zero, 1.5f);
        AnimateMenu(SettingsBackButton, new Vector3(0, -80, 0), 1f);
    }

    private void SettingsOut()
    {
        AnimateMenu(SettingsPanel, new Vector3(0, -1500, 0), 3f);
        AnimateMenu(SettingsGamePadButton, new Vector3(0, -1500, 0), 2f);
        AnimateMenu(SettingsKeyboardButton, new Vector3(0, -1500, 0), 1.5f);
        AnimateMenu(SettingsBackButton, new Vector3(0, -1500, 0), 1f);
    }

    private void AnimateMenu(GameObject obj, Vector3 position, float time)
    {
        if (obj)
            LeanTween.moveLocal(obj, position, time).setEase(LeanTweenType.easeOutElastic);
    }
}
