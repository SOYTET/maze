using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RelayManager : NetworkBehaviour
{
    public static string Public_JoinCodeDB = "maze";
    public string playerName;
    private UnityTransport _transpot;
    private const int maxPlayers = 4;
    private string _joinCodeText;
    // Notify state update
    public UnityAction<string> UpdateState;
    // Notify Match found
    public UnityAction FindJoin;
    public static bool isAlreadySignIn = false;

    async void Start()
    {
        _transpot = FindObjectOfType<UnityTransport>();
        await Authenticate();
        if (RoomUI.isCreateRoom)
        {
            CreateGame();
        }
        else if (RoomUI.isJoinRoom)
        {
            JoinGame();
        }
        // Subscribe to NetworkManager events
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
    }
    private void ClientConnected(ulong id)
    {
        Debug.Log("Connected player with id: " + id);
        UpdateState?.Invoke("Player found!");
        FindJoin?.Invoke();
    }
    private static async Task Authenticate()
    {
        if(!isAlreadySignIn)
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        isAlreadySignIn = true;
    }
    public async void CreateGame()
    {
        try
        {
            Allocation _a = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            _joinCodeText = await RelayService.Instance.GetJoinCodeAsync(_a.AllocationId);
            Public_JoinCodeDB = _joinCodeText;

            _transpot.SetHostRelayData(_a.RelayServer.IpV4, (ushort)_a.RelayServer.Port, _a.AllocationIdBytes, _a.Key, _a.ConnectionData);

        }
        catch
        {
            SceneManager.LoadScene("Home");
        }
        NetworkManager.Singleton.StartHost();
    }
    public async void JoinGame()
    {
        try
        {
            JoinAllocation _a = await RelayService.Instance.JoinAllocationAsync(RoomUI.JoinCode);
            _transpot.SetClientRelayData(_a.RelayServer.IpV4, (ushort)_a.RelayServer.Port, _a.AllocationIdBytes, _a.Key, _a.ConnectionData, _a.HostConnectionData);

        }
        catch
        {
            SceneManager.LoadScene("Home");
        }
        NetworkManager.Singleton.StartClient();
    }


}
