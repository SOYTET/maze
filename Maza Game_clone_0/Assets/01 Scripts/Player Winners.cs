using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using static PlayerInGame;
using Unity.Collections;

public class PlayerWinners : NetworkBehaviour
{
    public NetworkVariable<MyCustom> PlayerDBNetwork = new NetworkVariable<MyCustom>(default, readPerm:NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
    //private NetworkVariable<int> _playersWinner = new NetworkVariable<int  >(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] private GameObject PlayerInGameProfile;
    [SerializeField] private Transform ParentContainerName;

    //[SerializeField] private TMP_Text winnerCount;
    public static int WinnerCountPlayer;

    public static string PlayerWinnerDataBase;
    private string local_PlayerName;

    public struct MyCustom : INetworkSerializable
    {
        public FixedString128Bytes WinnerPlayerTemp;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref WinnerPlayerTemp);
        }
    }
    public override void OnNetworkSpawn()
    {
        PlayerDBNetwork.OnValueChanged += (MyCustom previousValue, MyCustom CurrentValue) =>
        {
            Debug.Log(CurrentValue.WinnerPlayerTemp);
            var instanceGO = Instantiate(PlayerInGameProfile, ParentContainerName);
            instanceGO.gameObject.GetComponentInChildren<TMP_Text>().text = CurrentValue.WinnerPlayerTemp.ToString();
        };
    }
    //public override void OnNetworkSpawn()
    //{
    //    _playersWinner.OnValueChanged += (int previouseValue , int CurrentValue) =>
    //    {
    //        Debug.Log("Player Winner: " + CurrentValue);
    //        var instanceGO = Instantiate(PlayerInGameProfile, ParentContainerName);
    //        instanceGO.gameObject.GetComponentInChildren<TMP_Text>().text = CurrentValue.ToString();
    //    };
    //}
    private void Start() 
    {
        local_PlayerName = Random.Range(0,1000).ToString();
    }
    bool mybool = true;
    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.T) & IsOwner)
        //{
        //    Debug.Log("Player Pressed");
        //    //_playersWinner.Value = ;
        //    //Debug.Log(NetworkManager.Singleton.LocalClientId);
        //}
        if (Input.GetKey(KeyCode.L))
        { 
            Debug.Log("server : "+IsServer + " ishost : " + IsHost+ " isclient : " + IsClient + " isowner : " + IsOwner + " IsLocalPlayer : " + IsLocalPlayer  );
        };
        if(IsOwner)
        {
            if (FPSController.isTriggerGoalAward)
            {
                if(mybool)
                {
                    mybool = false;
                    PlayerDBNetwork.Value = new MyCustom
                    {
                        WinnerPlayerTemp = RoomUI.PlayerName
                    };
                }
            }
        }
        if (Input.GetKey(KeyCode.K))
        {
            PlayerDBNetwork.Value = new MyCustom
            {
                WinnerPlayerTemp = Random.Range(0, 1000).ToString()
            };
        };


    }
}
