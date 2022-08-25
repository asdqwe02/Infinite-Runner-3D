using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using static Utility;
public class ResolutionEventArgs : EventArgs
{
    public int screenWidth;
    public int screenHeight;
    public bool fullScreen;
    public ResolutionEventArgs(int width, int height, bool fullScreen)
    {
        screenWidth = width;
        screenHeight = height;
        this.fullScreen = fullScreen;
    }
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool gameOver = false;
    public bool fullScreen;
    public bool pause;
    public uint score;
    public int screenWidth = 1920, screenHeight = 1080;
    public Transform gameOverScreen;
    public Transform pauseScreen;
    public Transform skillSelectScreen;
    public Transform scoreText;
    public Transform gameOverScoreText;
    public SettingData settingData;
    private Material _pauseScreenMaterial;
    public event EventHandler<ResolutionEventArgs> ResolutionChanged;
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
        fullScreen = Screen.fullScreen;
        if (pauseScreen)
            _pauseScreenMaterial = pauseScreen.GetComponent<Image>().material;
        LoadSettingData();
        expressionWeightPool.SortPool();
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
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            && pauseScreen != null
            && !gameOverScreen.gameObject.activeSelf
            && !skillSelectScreen.gameObject.activeSelf) // maybe change these to something more useful
        {
            TogglePause();
        }
        if (pause) // do this to have the pause screen shader effect
            _pauseScreenMaterial.SetFloat("_UnscaledTime", Time.unscaledTime);

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
    public void TogglePause()
    {
        Time.timeScale = Mathf.Abs(Time.timeScale - 1f);
        pause = !pause;
        pauseScreen.gameObject.SetActive(!pauseScreen.gameObject.activeSelf);
        if (pauseScreen.gameObject.activeSelf)
            AudioManager.instance.PauseAllSound();
        else AudioManager.instance.ResumeAllSound();

    }
    public void PlayButtonClickSound()
    {
        AudioManager.instance.PlaySound(AudioManager.Sound.ButtonClick);
    }
    public void RestartGame()
    {
        StopAllCoroutines();
        Time.timeScale = 1f;
        PlayButtonClickSound();
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
        PlayButtonClickSound();
        if (pause)
        {
            AudioManager.instance.RemoveSoundTrackFilter();
        }
        SceneManager.LoadScene(0);
        AudioManager.instance.PlaySoundTrack(AudioManager.SoundTrack.ST02);
    }
    public void SetResolution()
    {
        // Debug.Log($"fullscreen: {fullScreen}");
        // Debug.Log( $"resolution: {screenWidth}*{screenHeight}");
        Screen.SetResolution(screenWidth, screenHeight, fullScreen);
        OnResolutionChange(new ResolutionEventArgs(screenWidth, screenHeight, fullScreen));
    }
    public void ToggleFullScreen(bool fullScreen) // bug
    {
        Screen.fullScreenMode = fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        this.fullScreen = fullScreen;
        SetResolution();
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
    protected virtual void OnResolutionChange(ResolutionEventArgs e)
    {
        ResolutionChanged?.Invoke(this, e);
    }
}
