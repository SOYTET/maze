using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEditor;

public class RoomUI : NetworkBehaviour
{
    public static RoomUI Instance;
    public static bool isCreateRoom = false;
    public static bool isJoinRoom = false;
    public static string JoinCode = "default code";
    public static string PlayerName;
    public static string LobbyName;
    //Create Game
    public TMP_InputField PlayerNameHost;
    public TMP_InputField LobbyNameInput;

    //Join Game
    public TMP_InputField PlayerNameClient;
    public TMP_InputField JoinCodeInput;

    //container
    public GameObject JoinGameUI;
    public GameObject CreateGameUI;

    //button
    public GameObject CreateButton;
    public GameObject JoinButton;
    public GameObject BackButton;
    //public TMP_Text ma_log;


    //string
    public static string PlayerRoleStatus;

    //boolean
    public static bool IsFailJoin;

    //toggle
    [SerializeField]
    private Toggle IsPoliceToggle;

    void Awake()
    {
        JoinGameUI.SetActive(false);
        CreateGameUI.SetActive(false);
        CreateButton.SetActive(true);
        JoinButton.SetActive(true);
        JoinCodeInput.text = "Join Code...";
        BackButton.SetActive(false);
        
    }
    public void JoinGameMethodUI()
    {
        JoinGameUI.SetActive(true);
        CreateGameUI.SetActive(false);

        CreateButton.SetActive(false);
        JoinButton.SetActive(false);
        BackButton.SetActive(true);
    }
    public void CreateGameMethodUI()
    {
        JoinGameUI.SetActive(false);
        CreateGameUI.SetActive(true);

        CreateButton.SetActive(false);
        JoinButton.SetActive(false);
        BackButton.SetActive(true) ;
    }
    public void BackMethodUI()
    {
        JoinGameUI.SetActive(false);
        CreateGameUI.SetActive(false);

        CreateButton.SetActive(true);
        JoinButton.SetActive(true);
        BackButton.SetActive(false);
    }

    public void HostGameNow()
    {
        PlayerName = PlayerNameHost.text.ToString();
        LobbyName = LobbyNameInput.text.ToString();
        LobbyManager.Instance.CreateLobby();
        
        isCreateRoom = true;
        JoinGameUI.SetActive(false);
        CreateGameUI.SetActive(false);
        CreateButton.SetActive(false);
        JoinButton.SetActive(false);
        BackButton.SetActive(false);

        //if (IsPoliceToggle.isOn)
        //{
        //    PlayerRoleStatus = "BM";
        //}
        //else
        //{
        //    PlayerRoleStatus = "RN";
        //}
    }
    public void JoinGameNow()
    {
        PlayerName = PlayerNameClient.text.ToString();  
        JoinCode = JoinCodeInput.text.ToString();
        LobbyManager.Instance.JoinLobby();

        isJoinRoom = true;
        JoinGameUI.SetActive(false);
        CreateGameUI.SetActive(false);
        CreateButton.SetActive(false);
        JoinButton.SetActive(false);
        BackButton.SetActive(false);

    }
    public void onToggleChnage()
    {
        if (IsPoliceToggle.isOn)
        {
            PlayerRoleStatus = "BM";
            Debug.Log("Toggle is on");
        }
        else
        {
            PlayerRoleStatus = "RN";
            Debug.Log("Toggle is off");
        }
    }
    public void GoHome()
    {
        SceneManager.LoadScene("Home Screen");
    }


}
