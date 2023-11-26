using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [SerializeField]
    private Slider MusicSlider;
    [SerializeField]
    private AudioSource musicAudioClip;
    [SerializeField]
    private GameObject SettingPenel;
    void Start() {
        SettingPenel.SetActive(false);
    }
    void Update()
    {
        musicAudioClip.volume = MusicSlider.value;
    }
    public void ExitSetting()
    {
        SettingPenel.SetActive(false);
    }
    public void OpenSetting()
    {
        SettingPenel.SetActive(true);
    }
}
