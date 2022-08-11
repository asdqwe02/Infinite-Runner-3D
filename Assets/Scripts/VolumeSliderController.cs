using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class VolumeSliderController : MonoBehaviour
{
    [SerializeField] Slider slider;
    Action SetUp,sliderListener;
    public VolumeType volumeType;
    public enum VolumeType
    {
        MASTER,
        SOUNDTRACK,

    };
    private void Awake()
    {
        slider = GetComponent<Slider>();
        switch (volumeType)
        {
            case VolumeType.MASTER:
                SetUp = SetUpControlForMasterVolume;
                break;
            case VolumeType.SOUNDTRACK:
                SetUp = SetUpControlForSoundTrackVolume;
                break;
            default:
                break;
        }
    }
    private void Start()
    {
        SetUp();
        slider.onValueChanged.AddListener(delegate {sliderListener();});
    }
    public void UpdateMasterVolume()
    {
        AudioManager.instance.MVM = slider.value;
    }

    public void UpdateSoundTrackVolume()
    {
        AudioManager.instance.STV = slider.value;
    }

    public void SetUpControlForMasterVolume()
    {
        slider.value = AudioManager.instance.MVM;
        slider.onValueChanged.AddListener(delegate { UpdateMasterVolume(); });
        sliderListener = UpdateMasterVolume;
    }
    public void SetUpControlForSoundTrackVolume()
    {
        slider.value = AudioManager.instance.STV;
        slider.onValueChanged.AddListener(delegate { UpdateSoundTrackVolume(); });
        sliderListener = UpdateSoundTrackVolume;
    }
}
