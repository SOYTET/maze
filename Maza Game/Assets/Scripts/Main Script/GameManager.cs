using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;

public class GameManager : NetworkBehaviour
{


    [SerializeField]
    private float LocalHealth = 20f;


    public Lobby hostLobby, joinnedLobby;

    //shooting
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform bulletSpwan;
    [SerializeField]
    private Image ForGround_FillHealth;
    [SerializeField]
    private Image FillHealthUI;

    [SerializeField]
    private float bulletSpeed = 10f;



    //boolean
    private bool isHit = false;
    private bool isDead = false;
    private void Start()
    {
        
    }
    private void Update()
    {
        //HitHealthServerRpc();
        //if (Input.GetMouseButtonDown(0) && IsHost)
        //{
        //    ShootingServerRpc();
        //}

    }
    NetworkObject m_SpawnedNetworkObject;
    [ServerRpc]
    private void ShootingServerRpc()
    {
        var bullet = Instantiate(bulletPrefab, bulletSpwan.position, bulletSpwan.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpwan.forward * bulletSpeed;
        m_SpawnedNetworkObject = bullet.GetComponent<NetworkObject>();
        bullet.GetComponent<NetworkObject>().Spawn();
        //StartCoroutine(DespawnTimer());
    }
    //private IEnumerator DespawnTimer()
    //{
    //    yield return new WaitForSeconds(10);
    //    m_SpawnedNetworkObject.Despawn();
    //    yield break;
    //}



    [ServerRpc]
    private void HitHealthServerRpc()
    {
        ForGround_FillHealth.fillAmount = LocalHealth / 20f;
        FillHealthUI.fillAmount = LocalHealth / 20f;
        if (!IsOwner) return;
        if (isHit)
        {
            LocalHealth -= 1f;
            isHit = false;
            Debug.Log("health hit" + LocalHealth);
            if (LocalHealth <= 0)
            {
                isDead = true;
                Debug.Log("Player has been die!, isDead: " + isDead);
                NetworkManager.Singleton.Shutdown();
                SceneManager.LoadScene("Home");
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return;
        if (collision.gameObject.CompareTag("demage"))
        {
            isHit = true;
            Debug.Log("hit");
        }
    }


}
