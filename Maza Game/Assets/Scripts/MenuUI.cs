using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : NetworkBehaviour
{
    public void Level1(){
        SceneManager.LoadScene("Room");
    }
    public void LobbyMenu(){
        SceneManager.LoadScene("Level Menu");
    }
    public void home(){
        SceneManager.LoadScene("Home");
    }   
    public void ShutdownNetwork()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Home");
    }
}
