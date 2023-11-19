using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;


public class NetworkManagement : NetworkBehaviour
{
    public static NetworkVariable<bool> isImposterAsigned = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<int> PlayerID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    private void Awake()
    {
        FPSController.PublicPlayerID = PlayerID.Value;
        //ConnectionNotificationManager.Singleton.OnClientConnectionNotification += HandleClientConnectionNotification;
    }
    private void Update()
    {
        if (!IsServer) return;
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            PlayerID.Value++;
            //Debug.Log("client connected");
        };
        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            PlayerID.Value--;
            //Debug.Log("client disconnected");
        };
        if (Input.GetKey(KeyCode.F4))
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("Home");
        }
        //NetworkManager.Singleton.DisconnectClient
    }
    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
        // At this point we must use the UnityEngine's SceneManager to switch back to the MainMenu
        UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
    }
    //void HandleClientConnectionNotification(ulong clientId, ConnectionNotificationManager.ConnectionStatus status)
    //{
    //    if (status == ConnectionNotificationManager.ConnectionStatus.Disconnected)
    //    {
    //        NetworkManager.Singleton.Shutdown();
    //        SceneManager.LoadScene("Home");
    //    }
    //}

}
