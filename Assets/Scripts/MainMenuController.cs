using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    // [SerializeField] Transform mainMenu;
    // [SerializeField] Transform optionMenu;
    // [SerializeField] Transform helpMenu;
    [SerializeField] Transform pressAnyKeyText;

    [SerializeField] List<Transform> menus;
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
    private void Update() {
        if (Input.anyKey && !menus[0].gameObject.activeSelf)
        {
            pressAnyKeyText.gameObject.SetActive(false);
            menus[0].gameObject.SetActive(true);
        }

    }
}
