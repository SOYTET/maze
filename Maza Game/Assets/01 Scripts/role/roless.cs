using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class roless : NetworkBehaviour
{
    [SerializeField] private GameObject rolessPrefab;
    // Start is called before the first frame update
    public static bool box = false;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.M))
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
