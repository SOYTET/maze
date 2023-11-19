using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Netcode;
using System;

public class HomeManager : MonoBehaviour
{
    public TMP_InputField PlayerNameInput;
    public static string PlayerName = "Maze Mesh";
    //public static NetworkVariable<string> PlayerNameVariable;
    public static string PlayerNameVariable = "PlayerNameVariable";
    private void Start()
    {
        Cursor.visible = true;
    }
    void Update()
    {
        PlayerName = PlayerNameInput.text;
        
    }
    public void LobbyGameMode()
    {
        SceneManager.LoadScene("Level Menu");
    }


}