using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
[System.Serializable]
public class SettingData 
{
    public float masterVolume, musicVolume;
    public int screenWidth, screenHeight;
    public bool fullScreen;

    public SettingData(AudioManager audioManager, GameManager gameManager)
    {
        masterVolume = audioManager.MVM;
        musicVolume = audioManager.STV;
        screenWidth = gameManager.screenWidth;
        screenHeight = gameManager.screenHeight;
        fullScreen = gameManager.fullScreen;
    }
}