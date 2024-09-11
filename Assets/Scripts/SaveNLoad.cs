using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPos;
    public Vector3 playerRot;
}

public class SaveNLoad : MonoBehaviour
{

    private SaveData saveData = new SaveData();
    private string SAVE_DATA_DIRECTORY;
    private string SAVE_FILENAME = "/SaveFile.txt/";
    private PlayerMovement thePlayer;
    // Start is called before the first frame update
    void Start()
    {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Saves/";

        if(Directory.Exists(SAVE_DATA_DIRECTORY))
        Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
    }
    public void SaveData()
    {
        thePlayer = FindObjectOfType<PlayerMovement>();
        saveData.playerPos = thePlayer.transform.position;
        saveData.playerRot = thePlayer.transform.eulerAngles;
        string json = JsonUtility.ToJson(saveData); //플레이어의 위치를 제이슨화시킴
        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);
        Debug.Log("저장 완료");
        Debug.Log(json);
    }
    public void LoadData()
    {
        if(File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
        {
        string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
        saveData = JsonUtility.FromJson<SaveData>(loadJson);
        thePlayer = FindObjectOfType<PlayerMovement>();
        thePlayer.transform.position = saveData.playerPos;
        thePlayer.transform.eulerAngles = saveData.playerRot;
        Debug.Log("로드 완료");
        }
        else
        Debug.Log("세이브 파일이 없습니다.");
    }
}