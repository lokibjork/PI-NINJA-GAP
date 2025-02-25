using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public SaveData[] saveSlots = new SaveData[3]; // Três slots de save

    private string savePath;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSaveData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSaveData()
    {
        savePath = Application.persistentDataPath + "/saveSlot_";

        // Tenta carregar cada slot de save
        for (int i = 0; i < saveSlots.Length; i++)
        {
            saveSlots[i] = LoadGame(i) ?? new SaveData();
        }
    }

    public void SaveGame(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= saveSlots.Length) return;

        saveSlots[slotIndex].hasSave = true;

        string filePath = savePath + slotIndex + ".dat";
        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream file = File.Create(filePath))
        {
            formatter.Serialize(file, saveSlots[slotIndex]);
        }

        Debug.Log($"Game saved in slot {slotIndex}");
    }

    public SaveData LoadGame(int slotIndex)
    {
        string filePath = savePath + slotIndex + ".dat";

        if (!File.Exists(filePath)) return null;

        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream file = File.Open(filePath, FileMode.Open))
        {
            return (SaveData)formatter.Deserialize(file);
        }
    }

    public void DeleteSave(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= saveSlots.Length) return;

        string filePath = savePath + slotIndex + ".dat";

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            saveSlots[slotIndex] = new SaveData();
            Debug.Log($"Save deleted from slot {slotIndex}");
        }
    }
}
