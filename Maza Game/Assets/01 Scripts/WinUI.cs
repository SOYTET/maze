using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinUI : MonoBehaviour
{
    public TMP_Text WinnerPlayer;
    public static string winner = "winner ui";
    // public static NetworkVariable<string> playerCount = new NetworkVariable<string>(1, NetworkVariableReadPermission.Everyone);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        WinnerPlayer.text = winner;
    }
}
