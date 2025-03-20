using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private string _path;
    private BinaryFormatter formatter = new BinaryFormatter();

    [SerializeField] private UIInteractions _UI;
    [SerializeField] private GameObject _noAdButton;

    [Serializable]
    public class PlayerInfo
    {
        public int BestScore = 0;
        public string UniqueID = "";
        public bool AdInfo = false;
        public string Username = "Username";
        public int PlayCount = 0;
    }

    public PlayerInfo playerInfo = new PlayerInfo();

    private void Awake()
    {
        _path = Application.persistentDataPath + "Memorytesterdatafile3445.dat";
    }
    private void Start()
    {

        if (File.Exists(_path))
        {
            LoadData(); //load data everytime you login.
        }
        else
        {
            playerInfo.BestScore = 0;
            playerInfo.UniqueID = "abcde12345";
            playerInfo.AdInfo = false;
            playerInfo.Username = "Username";
            playerInfo.PlayCount = 0;
            SaveData();
            LoadData();
        }      
    }
    public void SaveData() //save to local data when you close application.
    {
        FileStream fs = new FileStream(_path, FileMode.Create);
        using (fs)
        {
            formatter.Serialize(fs, playerInfo);
            Debug.Log("Data saved to local.");
        }
    }
    public void LoadData()
    {
        FileStream fs = new FileStream(_path, FileMode.Open);
        playerInfo = (PlayerInfo)formatter.Deserialize(fs);
        fs.Close();

        _UI.SetBestScore(playerInfo.BestScore);
        if(playerInfo.AdInfo) _noAdButton.SetActive(false);
    }
    private void OnApplicationQuit()
    {
        SaveData();
    }
    public void UpdateScore(int score)
    {
        if (score > playerInfo.BestScore) 
        {
            playerInfo.BestScore = score;
            SaveData();
            _UI.SetBestScore(playerInfo.BestScore);
        }
        
    }

    public void NoAdFeaturePurchased()
    {
        playerInfo.AdInfo = true;
        SaveData();
        _noAdButton.SetActive(false);
    }

    public bool GetAdInfo() { return playerInfo.AdInfo; }
}
