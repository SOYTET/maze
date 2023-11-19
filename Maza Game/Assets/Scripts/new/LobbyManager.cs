using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;

public class LobbyManager : NetworkBehaviour
{
    //public static string PlayerName_Local;
    public static LobbyManager Instance;

    public string playerNameInput;
    public string lobbyCodeInput;

    public static Lobby hostLobby, joinnedLobby;
    public GameObject CameraTemp;

	public TMP_Text[] lobbyPlayersText;

    public TMP_Text PlayerNameDisplayPrefab;
    public Transform ParentContainerName;

	public TMP_Text lobbyCodeText;

    public NetworkVariable<int> PlayerInGame = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> CloseGame = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);

    //public static NetworkVariable<Dictionary<int, string>> MyPlayerWinner = new NetworkVariable<Dictionary<int, string>>(writePerm: NetworkVariableWritePermission.Server);


    public GameObject startGameButton;

    public static bool startedGame;

	// Start is called before the first frame update
	async void Start()
    {
        Instance = this;

        await UnityServices.InitializeAsync();
        playerNameInput = HomeManager.PlayerName;
        lobbyCodeInput = RoomUI.JoinCode;
        startGameButton.SetActive(false);

        if (RoomUI.isCreateRoom)
        {
            CreateLobby();
        }
        else if (RoomUI.isJoinRoom)
        {
            JoinLobby();
        }
        Debug.Log(NetworkManager.Singleton.LocalClientId);
    }


    async Task Authenticate()
    {

		if (AuthenticationService.Instance.IsSignedIn)
		{
			return;
		}

        AuthenticationService.Instance.ClearSessionToken();

		AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Logado como " + AuthenticationService.Instance.PlayerId);
        };        

		await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    async public void CreateLobby()
    {
        try
        {

            await Authenticate();

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {"StartGame", new DataObject(DataObject.VisibilityOptions.Member, "0") }
                }
            };

			Lobby lobby = await Unity.Services.Lobbies.LobbyService.Instance.CreateLobbyAsync("Lobby", 4, createLobbyOptions);

            Debug.Log("Criou o lobby " + lobby.LobbyCode);

			hostLobby = lobby;
            joinnedLobby = hostLobby;
			lobbyCodeText.text = lobby.LobbyCode;
            startGameButton.SetActive(true);
			ShowPlayersOnLobby();
			InvokeRepeating("LobbyHeartBeat", 5, 5);
            InvokeRepeating("LobbyUpdateHeartBeat", 4, 4);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }


    }
      

    void CheckForLobbyUpdates()
    {
        if(joinnedLobby == null || startedGame)
        {
            return;
        }

        UpdateLobby();
        ShowPlayersOnLobby();
        if (joinnedLobby.Data["StartGame"].Value != "0")
        {
            if(hostLobby == null)
            {
                JoinRelay(joinnedLobby.Data["StartGame"].Value);
            }
            startedGame = true;
        }
    }
    private bool isasignUpdateAlready = false;
    void LobbyUpdateHeartBeat()
    {
        
        if (joinnedLobby == null || startedGame)
        {
            return;
        }
        if (FPSController.isTriggerGoalAward)
        {
            //PlayerDeadUpdateStatus();
            //PlayerDeadUpdateStatus();
            if (IsOwner && !isasignUpdateAlready)
            {
                UpdatePlayer();
                isasignUpdateAlready = true;
            }
        }
        else
        {

        }
    }
    async void LobbyHeartBeat()
    {
        if (hostLobby == null)
            return;

            await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
        Debug.Log("Heartbeat lobby");
        UpdateLobby();
		ShowPlayersOnLobby();
	}
    
    async public void JoinLobby()
    {
        try
        {

            await Authenticate();

            JoinLobbyByCodeOptions createLobbyOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

			Lobby lobby = await Unity.Services.Lobbies.LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCodeInput, createLobbyOptions);

            joinnedLobby = lobby;

			lobbyCodeText.text = lobby.LobbyCode;
            startGameButton.SetActive(false);

			Debug.Log("Joined lobby " + lobby.LobbyCode);

			ShowPlayersOnLobby();
            InvokeRepeating("CheckForLobbyUpdates", 3, 3);
		}
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    Player GetPlayer()
    {
        Player player = new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerNameInput) }
                    }
        };
      
        return player;
    }

    async void UpdateLobby()
    {
		if (joinnedLobby == null)
			return;

        joinnedLobby = await LobbyService.Instance.GetLobbyAsync(joinnedLobby.Id);
        
	}

	void ShowPlayersOnLobby()
	{
        foreach(Transform Child in ParentContainerName)
        {
            Destroy(Child.gameObject);
        }
        for (int i = 0; i < joinnedLobby.Players.Count; i++)
        {
            string[] playerNames = joinnedLobby.Players[i].Data["name"].Value.ToString().Split(','); // Assuming names are comma-separated
            foreach (string playerName in playerNames)
            {
                var instanceGO = Instantiate(PlayerNameDisplayPrefab, ParentContainerName);
                instanceGO.text = playerName;
            }
        }

    }


    async Task<string> CreateRelay()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        //disable camera before game start
        CameraTemp.SetActive(false);
        startGameButton.SetActive(false);
        NetworkManager.Singleton.StartHost();


        return joinCode;
    }

    async void JoinRelay(string joinCode)
    {
        Debug.Log("Criando relay");
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        //disable camera before game start
        CameraTemp.SetActive(false);
        NetworkManager.Singleton.StartClient();
    }

    

    public async void StartGame()
    {
        string relayCode = await CreateRelay();


        Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinnedLobby.Id, new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                {"StartGame", new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
            }
        });

        joinnedLobby = lobby;

    }

    public async void UpdatePlayer()
    {

        Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinnedLobby.Id, new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                {"PlayerName", new DataObject(DataObject.VisibilityOptions.Member, PlayerWinners.PlayerWinnerDataBase + HomeManager.PlayerName) }
            }
        });
        Debug.Log(lobby.Data["PlayerName"].Index +" : " + lobby.Data["PlayerName"].Value);
        joinnedLobby = lobby;
    }
    public  void PlayerDeadUpdateStatus()
    {
        //Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinnedLobby.Id, new UpdateLobbyOptions
        //{
        //    Data = new Dictionary<string, DataObject>
        //    {
        //        {"PlayerName", new DataObject(DataObject.VisibilityOptions.Member, HomeManager.PlayerName + "has been died!") }
        //    }
        //});
        //Debug.Log(lobby.Data["PlayerName"].Index + " : " + lobby.Data["PlayerName"].Value);
        //joinnedLobby = lobby;
    }


    void Update()
    {
        
        if (!IsServer) return;
        PlayerInGame.Value = NetworkManager.Singleton.ConnectedClients.Count;

    }
   




}
