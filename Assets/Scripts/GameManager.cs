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
        scoreText.GetComponent<TextMeshProUGUI>().text = score.ToString();
        gameOver = false;
    }
    private void Update()
    {
        if (PlayerController.instance.totalPowerLevel <= 0 && !gameOver)
        {
            AudioManager.instance.PlaySoundTrack(AudioManager.SoundTrack.ST03_1);
            gameOver = true;
            gameOverScreen.gameObject.SetActive(true);
            StartCoroutine(PauseGame(1f));
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
        AudioManager.instance.PlaySoundTrack(AudioManager.SoundTrack.ST02);
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
}
