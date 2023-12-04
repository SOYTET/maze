using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TargetShooting : NetworkBehaviour
{
    public float health = 200f;
    public Image ForGround_FillHealth;
    public Image FillHealthUI;
    public static bool isTargetDead;

    public GameObject ImpactDead;

    private void Update()
    {
        UpdateHealthServerRpc();
    }
    public void TakeDemage(float amount)
    {
        health -= amount;
        if (health < 0)
        {
            dieServerRpc();
            Debug.Log(health);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void dieServerRpc()
    {
        dieClientRpc();
    }
    [ClientRpc]
    void dieClientRpc()
    {
        Instantiate(ImpactDead, transform.position, Quaternion.identity);
        gameObject.transform.position = new Vector3(0f, 10f, 0f);
        health = 200f;
    }
    [ServerRpc (RequireOwnership =false)]
    void UpdateHealthServerRpc()
    {
        UpdateHealthClientRpc();
    }
    [ClientRpc]
    void UpdateHealthClientRpc()
    {
        ForGround_FillHealth.fillAmount = health / 200f;
        FillHealthUI.fillAmount = health / 200f;
    }
}
