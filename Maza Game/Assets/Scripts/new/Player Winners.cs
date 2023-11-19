using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;

public class PlayerWinners : NetworkBehaviour
{

    [SerializeField] private GameObject PlayerInGameProfile;
    [SerializeField] private Transform ParentContainerName;

    [SerializeField] private TMP_Text winnerCount;
    public static int WinnerCountPlayer;

    public static string PlayerWinnerDataBase;

    private void Start()
    {
        InvokeRepeating("ShowWinnersPlayer", 3, 3);
    }

    void ShowWinnersPlayer()
    {
        //if(LobbyManager.MyPlayerWinner.Values != null)
        // {
        if (IsOwner)
        {

            //winnerCount.text = WinnerCountPlayer.ToString();
            //PlayerWinnerDataBase = LobbyManager.joinnedLobby.Data["PlayerName"].Value.ToString();
            //Debug.Log("Player Winner Count" + WinnerCountPlayer.ToString());
            //if (LobbyManager.joinnedLobby.Data["PlayerName"].Value != null)
            //{
            //    foreach (Transform Child in ParentContainerName)
            //    {
            //        Destroy(Child.gameObject);
            //    }
            //    Debug.Log(PlayerWinnerDataBase);
            //    for (int i = 0; i < LobbyManager.joinnedLobby.Players.Count; i++)
            //    {
            //        string[] playerNames = LobbyManager.joinnedLobby.Data["PlayerName"].Value.ToString().Split(','); // Assuming names are comma-separated
            //        foreach (string playerName in playerNames)
            //        {
            //            var instanceGO = Instantiate(PlayerInGameProfile, ParentContainerName);
            //            instanceGO.gameObject.GetComponentInChildren<TMP_Text>().text = playerName;
            //        }
            //    }
            //}
            if (LobbyManager.joinnedLobby.Data["PlayerName"].Value != null)
            {
                foreach (Transform Child in ParentContainerName)
                {
                    Destroy(Child.gameObject);
                }
                foreach (var PlayerWinner in LobbyManager.joinnedLobby.Data["PlayerWinner"].Value)
                {
                    var instanceGO = Instantiate(PlayerInGameProfile, ParentContainerName);
                    instanceGO.gameObject.GetComponentInChildren<TMP_Text>().text = PlayerWinner.ToString();
                }

            }
        }

    }

}
