using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingUI : MonoBehaviour
{
    public GameObject loadingUI;
    // Start is called before the first frame update
    void Start()
    {
        loadingUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (QuickJoin.isCreateMatch || QuickJoin.isFoundMatch)
        {
            loadingUI.SetActive(false);
        }
    }
}
