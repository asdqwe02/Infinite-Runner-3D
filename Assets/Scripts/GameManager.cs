using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool gameOver = false;
    public uint score;
    public Transform gameOverScreen;
    public Transform scoreText;
    public Transform gameOverScoreText;
    [SerializeField] int _screenWidth = 1920, _screenHeight = 1080;
    public bool fullScreen;
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
    public void RestartGame()
    {
        StopAllCoroutines();
        Time.timeScale = 1f;
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
        SceneManager.LoadScene(0);
        AudioManager.instance.PlaySoundTrack(AudioManager.SoundTrack.ST02);
    }
    public void SetResolution()
    {
        Screen.SetResolution(_screenWidth, _screenHeight, false);
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
        // Debug.Log(Screen.fullScreen);
    }
    public void SetResWidth(int width)
    {
        _screenWidth = width;
    }
    public void SetResHeight(int height)
    {
        _screenHeight = height;
    }
}
