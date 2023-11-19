using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;

public class PlayerInGame : NetworkBehaviour
{

    [SerializeField] private GameObject PlayerInGameProfile;
    [SerializeField] private Transform ParentContainerName;

    private void Start()
    {
        InvokeRepeating("ShowPlayersOnLobby", 3, 3);
    }

    void ShowPlayersOnLobby()
    {
        if (LobbyManager.joinnedLobby.Players != null)
        {
            foreach (Transform Child in ParentContainerName)
            {
                Destroy(Child.gameObject);
            }
            for (int i = 0; i < LobbyManager.joinnedLobby.Players.Count; i++)
            {
                string[] playerNames = LobbyManager.joinnedLobby.Players[i].Data["name"].Value.ToString().Split(','); // Assuming names are comma-separated
                foreach (string playerName in playerNames)
                {
                    var instanceGO = Instantiate(PlayerInGameProfile, ParentContainerName);
                    instanceGO.gameObject.GetComponentInChildren<TMP_Text>().text = playerName;
                    Debug.Log("Player In Game: " + playerName);
                }
            }
        }
    }
}
