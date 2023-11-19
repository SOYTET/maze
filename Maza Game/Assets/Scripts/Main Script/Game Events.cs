using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEvents : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.F4))
        {
            ExitGamePlay();
        }
    }
    private void ExitGamePlay()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Home");
    }
}
