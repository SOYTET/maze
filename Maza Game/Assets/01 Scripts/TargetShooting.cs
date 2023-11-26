using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetShooting : MonoBehaviour
{
    public float health = 50f;
    public void TakeDemage(float amount)
    {
        health -= amount;
        if(health < 0)
        {
            die();
        }
    }
    void die()
    {
        Destroy(gameObject);
    }
}
