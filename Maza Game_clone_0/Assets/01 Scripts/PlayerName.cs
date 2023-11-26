using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
	
	public TMP_Text playerNameText;
	LobbyManager lobbyManager;

    [SerializeField] private GameObject rolessPrefab;

    private IEnumerator Start()
	{
		playerNameText = GetComponentInChildren<TMP_Text>();

		lobbyManager = FindObjectOfType<LobbyManager>();


        if (IsServer)
		{
			while (NetworkManager.Singleton.ConnectedClients.Count != LobbyManager.joinnedLobby.Players.Count)
			{
				yield return new WaitForSeconds(1);
			}
			yield return new WaitForSeconds(1);
			for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
			{
				NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject.GetComponentInChildren<PlayerName>()
					.SetPlayerNameClientRpc(LobbyManager.joinnedLobby.Players[i].Data["name"].Value);
				NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject.GetComponentInChildren<PlayerName>().SetPlayerRoleClientRpc(LobbyManager.joinnedLobby.Players[i].Data["role"].Value);

            }
		}
	}



	[ServerRpc]
	public void SetPlayerNameServerRpc()
	{		
		//SetPlayerNameClientRpc(lobbyManager.playerNameInput.text);
	}

	[ClientRpc]
	public void SetPlayerNameClientRpc(string playerName)
	{
		playerNameText.text = playerName;
	}
    [ClientRpc]
    public void SetPlayerRoleClientRpc(string playerRoleClient)
    {
		Debug.Log(playerRoleClient);
 
        if (playerRoleClient == "BM")
        {
            if (IsOwner)
            {
              PoliceServerRpc();
            }
        }
    }
    [ServerRpc]
    private void PoliceServerRpc()
    {
        PoliceClientRpc();
    }
    [ClientRpc]
    private void PoliceClientRpc()
    {
        rolessPrefab.SetActive(true);
    }
}
