using System;
using System.IO;
using UnityEngine;

public class DataSave : MonoBehaviour
{
    void SaveData()
    {
        SaveDataModel saveManager = new SaveDataModel();
        
        saveManager.playerName = "Dev Ryu";
        saveManager.playerLevel = 1;

        string json = JsonUtility.ToJson( saveManager );
        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
        Debug.Log( "Writing file to: " + Application.persistentDataPath);
    }
}
[Serializable]
public class SaveDataModel
{
    public string playerName;
    public int playerLevel;
}