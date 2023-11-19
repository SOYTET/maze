using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class Level1NetworkUI : NetworkBehaviour
{
    public TMP_Text playerCount;
    public Animator Amt_door;
    public static  NetworkVariable<bool> StartGame = new NetworkVariable<bool>();
    public NetworkVariable<int> PlayerInGame = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    void Update()
    {
        if(StartGame.Value)
        {
            Amt_door.SetBool("isDoorOpen", true);
        }
        //count player
        playerCount.text = "Active : " + PlayerInGame.Value.ToString();
        if (!IsServer) return;
         PlayerInGame.Value = NetworkManager.Singleton.ConnectedClients.Count;
       // NetworkManager.Singleton.ConnectedClientsList
    }

}
