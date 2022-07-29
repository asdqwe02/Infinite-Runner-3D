using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Transform LoseMessage;
    private void Awake() {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Update() 
    {
        if (PlayerController.instance.totalPowerLevel <=0)
        {
            LoseMessage.gameObject.SetActive(true);
            PauseGame();
        }

    }
    public  void PauseGame ()
    {
        Time.timeScale = 0;
    }
}
