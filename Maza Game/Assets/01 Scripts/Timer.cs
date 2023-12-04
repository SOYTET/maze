using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class Timer : NetworkBehaviour
{
    [SerializeField] public TextMeshProUGUI timerText;
    public float remainingTime;
    public static bool isTimeOut;
    private bool isStartTime;
    public override void OnNetworkSpawn()
    {
        isStartTime = true;
    }
    void Update()
    {
        if (isStartTime)
        {
            remainingTime -= Time.deltaTime;
            int minute = Mathf.FloorToInt(remainingTime / 60);
            int second = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minute, second);
        }
        if (remainingTime < 1)
        {
            isTimeOut = true;
            remainingTime = 600f;
            Debug.Log("Time is out");
        }
    }
}
