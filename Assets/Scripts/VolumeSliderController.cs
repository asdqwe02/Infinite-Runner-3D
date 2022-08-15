using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class VolumeSliderController : MonoBehaviour
{
    [SerializeField] Slider slider;
    Action SetUp, sliderListener;
    public VolumeType volumeType;
    public TextMeshProUGUI valueText;
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
        slider.onValueChanged.AddListener(delegate { sliderListener(); });
    }
    private void OnEnable()
    {
    }
    public void UpdateMasterVolume()
    {
        AudioManager.instance.MVM = slider.value;
        valueText.text = Math.Round(slider.value * 100f, 1).ToString() + "%";
        // valueText.text = (slider.value * 100).ToString(".#") + "%";

    }

    public void UpdateSoundTrackVolume()
    {
        AudioManager.instance.STV = slider.value;
        valueText.text = Math.Round(slider.value * 100f, 1).ToString() + "%";
        // valueText.text = (slider.value * 100).ToString(".#") + "%";
    }

    public void SetUpControlForMasterVolume()
    {
        slider.value = AudioManager.instance.MVM;
        slider.onValueChanged.AddListener(delegate { UpdateMasterVolume(); });
        sliderListener = UpdateMasterVolume;
        valueText.text = Math.Round(slider.value * 100f, 1).ToString() + "%";

    }
    public void SetUpControlForSoundTrackVolume()
    {
        slider.value = AudioManager.instance.STV;
        slider.onValueChanged.AddListener(delegate { UpdateSoundTrackVolume(); });
        sliderListener = UpdateSoundTrackVolume;
        valueText.text = Math.Round(slider.value * 100f, 1).ToString() + "%";

    }
}
