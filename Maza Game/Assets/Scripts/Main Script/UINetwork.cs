using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.SceneManagement;

public class UINetwork : NetworkBehaviour
{
    [SerializeField]
    private TMP_Text joincodeUI;  
    void Update()
    {
        joincodeUI.text = RelayManager.Public_JoinCodeDB;
        if(Input.GetKey(KeyCode.Delete))
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("Home");
        }
    }
}
