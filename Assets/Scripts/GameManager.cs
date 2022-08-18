using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool gameOver = false;
    public uint score;
    public Transform gameOverScreen;
    public Transform pauseScreen;
    public Transform scoreText;
    public Transform gameOverScoreText;
    public int screenWidth = 1920, screenHeight = 1080;
    public bool fullScreen;
    public SettingData settingData;
    public Action TogglePause;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        score = 0;
        if (scoreText)
            scoreText.GetComponent<TextMeshProUGUI>().text = score.ToString();
        gameOver = false;
        TogglePause = PauseGame;
        LoadSettingData();
    }
    private void Update()
    {
        if (PlayerController.instance)
        {
            if (PlayerController.instance.totalPowerLevel <= 0 && !gameOver)
            {
                AudioManager.instance.PlaySoundTrack(AudioManager.SoundTrack.ST03_1);
                gameOver = true;
                gameOverScreen.gameObject.SetActive(true);
                StartCoroutine(PauseGame(1f));
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) && pauseScreen!=null)
        {
            TogglePause();
        }

    }
    public IEnumerator PauseGame(float slowDuration)
    {
        gameOverScoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + score.ToString();
        scoreText.gameObject.SetActive(false);
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = Time.timeScale * Time.deltaTime;
        yield return new WaitForSeconds(slowDuration);
        Time.timeScale = 0;
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseScreen.gameObject.SetActive(true);
        TogglePause = UnpauseGame;
    }
    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        pauseScreen.gameObject.SetActive(false);
        TogglePause = PauseGame;
    }
    public void RestartGame()
    {
        StopAllCoroutines();
        Time.timeScale = 1f;
        AudioManager.instance.PlaySound(AudioManager.Sound.ButtonClick);
        // AudioManager.instance.PlaySoundTrack(AudioManager.SoundTrack.ST02);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
    public void AddScore(uint Score)
    {
        score += Score;
        scoreText.GetComponent<TextMeshProUGUI>().text = score.ToString();

    }
    public void BackToMainMenu()
    {
        StopAllCoroutines();
        Time.timeScale = 1f;
        AudioManager.instance.PlaySound(AudioManager.Sound.ButtonClick);
        SceneManager.LoadScene(0);
        AudioManager.instance.PlaySoundTrack(AudioManager.SoundTrack.ST02);
    }
    public void SetResolution()
    {
        Screen.SetResolution(screenWidth, screenHeight, fullScreen);
    }
    public void ToggleFullScreen()
    {
        if (Screen.fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.fullScreen = false;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Screen.fullScreen = true;
        }
        fullScreen = Screen.fullScreen;
        // Debug.Log(Screen.fullScreen);
    }
    public void SetResWidth(int width)
    {
        screenWidth = width;
    }
    public void SetResHeight(int height)
    {
        screenHeight = height;
    }
    public void LoadSettingData()
    {
        settingData = SaveSystem.LoadSettingData();
        if (settingData != null)
        {
            screenHeight = settingData.screenHeight;
            screenWidth = settingData.screenWidth;
            fullScreen = settingData.fullScreen;
            SetResolution();
        }
    }
    public void SaveSettingData()
    {
        SaveSystem.SaveSetting(AudioManager.instance, GameManager.instance);
    }
}
