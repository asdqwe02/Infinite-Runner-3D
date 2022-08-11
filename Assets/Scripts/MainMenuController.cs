using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Transform mainMenu;
    [SerializeField] Transform optionMenu;
    [SerializeField] Transform pressAnyKeyText;

    List<Transform> menus;
    private void Awake()
    {
        menus = new List<Transform>(){
            mainMenu,
            optionMenu,
        };
    }
    public void ShowMenu(int index)
    {
        AudioManager.instance.PlaySound(AudioManager.Sound.ButtonClick);
        menus[index].gameObject.SetActive(true);
    }
    public void HideMenu(int index)
    {
        AudioManager.instance.PlaySound(AudioManager.Sound.ButtonClick);
        menus[index].gameObject.SetActive(false);
    }
    public void ShowPressAnyKeyText()
    {
        
    }
    private void Update() {
        if (Input.anyKey && !mainMenu.gameObject.activeSelf)
        {
            pressAnyKeyText.gameObject.SetActive(false);
            mainMenu.gameObject.SetActive(true);
        }

    }
}
