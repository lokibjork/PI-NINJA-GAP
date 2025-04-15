using UnityEngine;
using UnityEditor;

public class PlayerPrefsTools
{
    [MenuItem("Tools/Reset PlayerPrefs")]
    public static void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Todos os PlayerPrefs foram deletados!");
    }
}
