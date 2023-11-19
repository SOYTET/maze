using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    public static bool isCreateRoom = false;
    public static bool isJoinRoom = false;
    public static string JoinCode = "default code";
    public TMP_InputField JoinCodeInput;
    public GameObject JoinCodeUI;
    public GameObject CreateRoomButton;
    public GameObject CreateButton;
    public GameObject JoinRoomButton;
    public GameObject JoinButton;
    public TMP_Text ma_log;

    public GameObject Back;

    void Awake()
    {
        CreateRoomButton.SetActive(false);
        CreateButton.SetActive(true);
        JoinRoomButton.SetActive(false);
        JoinButton.SetActive(true);
        // JoinRoomByCode.enabled = false;
        // JoinCode.enabled = false;
        Back.SetActive(false);
        JoinCodeUI.SetActive(false);
        JoinCodeInput.text = "Join Code...";
    }
    public void CreateMethod()
    {
        CreateRoomButton.SetActive(true);
        CreateButton.SetActive(false);
        JoinRoomButton.SetActive(false);
        JoinButton.SetActive(false);
        Back.SetActive(true);
    }
    public void CreateRoomMethod()
    {
        CreateRoomButton.SetActive(false);
        CreateButton.SetActive(false);
        JoinRoomButton.SetActive(false);
        JoinButton.SetActive(false);

        isCreateRoom = true;
        SceneManager.LoadScene("Level I");
    }
    public void JoinMethod()
    {
        CreateRoomButton.SetActive(false);
        CreateButton.SetActive(false);
        JoinRoomButton.SetActive(true);
        JoinButton.SetActive(false);
        Back.SetActive(true);

        JoinCodeUI.SetActive(true);
    }
    public void JoinRoomMethod()
    {
        CreateRoomButton.SetActive(false);
        CreateButton.SetActive(false);
        JoinRoomButton.SetActive(false);
        JoinButton.SetActive(false);
        JoinCode = JoinCodeInput.text;

        SceneManager.LoadScene("Level I");
        isJoinRoom = true;
    }
    public void BackButton()
    {
        Back.SetActive(false);

        CreateRoomButton.SetActive(false);
        CreateButton.SetActive(true);
        JoinRoomButton.SetActive(false);
        JoinButton.SetActive(true);
        JoinCodeUI.SetActive(false);
    }


}
