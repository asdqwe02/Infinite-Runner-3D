using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool gameOver = false;
    public Transform gameOverScreen;
    private void Awake() {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        gameOver = false;
    }
    private void Update() 
    {
        if (PlayerController.instance.totalPowerLevel <=0 && !gameOver)
        {
            gameOver = true;
            gameOverScreen.gameObject.SetActive(true);
            StartCoroutine(PauseGame(1f));
        }

    }
    public IEnumerator PauseGame (float slowDuration)
    {
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = Time.timeScale *Time.deltaTime;
        yield return new WaitForSeconds(slowDuration);
        Time.timeScale = 0;
    }
    public void RestartGame()
    {
        StopAllCoroutines();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
