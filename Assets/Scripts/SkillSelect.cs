using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillSelect : MonoBehaviour
{
    public Transform skillUI;
    public List<Sprite> skillIcon;

    void Start()
    {
        Time.timeScale = 0f;
    }
    public void SelectLaserSkill()
    {
        PlayerController.instance.SetUpLaserSkill();
        ChangeSkillUI(0);
        skillUI.gameObject.SetActive(true);
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
    public void SelectShieldSkill()
    {
        PlayerController.instance.SetUpShieldSkill();
        ChangeSkillUI(1);

        skillUI.gameObject.SetActive(true);
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
    public void SelectBombSkill()
    {
        PlayerController.instance.SetUpBombSkill();
        ChangeSkillUI(2);
        skillUI.gameObject.SetActive(true);
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
    public void ChangeSkillUI(int index)
    {
        skillUI.GetComponent<Image>().sprite = skillIcon[index];
    }
    private void OnDisable() {
        SpawnInitialEnnemy();
    }
    public void SpawnInitialEnnemy()
    {
        LevelManager.instance.GetCurrentLevel().GetComponentInChildren<PlateController>().currentEnenmySpawner.SpawnEnemyEntity(); // bad fix 
    }
}
