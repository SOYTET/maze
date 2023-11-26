using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShootingPrefab : NetworkBehaviour
{
    private bool shouldDestroy = false;

    private void Update()
    {
        if (shouldDestroy && IsServer)
        {
            DestroyGameObjectServerRpc();
        }
    }

    [ServerRpc]
    private void DestroyGameObjectServerRpc()
    {
        // To avoid unnecessary RPC calls and potential synchronization issues,
        // check if the object hasn't been destroyed yet
        DestroyGameObjectClientRpc();
      
    }
    [ClientRpc]
    private void DestroyGameObjectClientRpc()
    {
        if (IsServer && IsOwner)
        {
            NetworkObject.Destroy(gameObject);
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    shouldDestroy = true;

    //    // To ensure the trigger doesn't affect objects not owned by the current client
    //    if (IsServer && IsOwner)
    //    {
    //        DestroyGameObjectServerRpc();
    //    }
    //}
    private void OnCollisionExit(Collision collision)
    {
        shouldDestroy = true;

        // To ensure the trigger doesn't affect objects not owned by the current client
        if (IsServer && IsOwner)
        {
            DestroyGameObjectServerRpc();
        }
    }
}
