using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillSelect : MonoBehaviour
{
    public Transform skillUI;
    public List<Sprite> skillIcon;
    // Start is called before the first frame update
    void Start()
    {
        // skillUI.gameObject.SetActive(false);
        Time.timeScale = 0f;
        // skillUI.gameObject.SetActive(false);
    }
    public void SelectLaserSkill()
    {
        PlayerController.instance.SetUpLaserSkill();
        skillUI.GetComponent<Image>().sprite = skillIcon[0];
        skillUI.gameObject.SetActive(true);
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
    public void SelectShieldSkill()
    {
        PlayerController.instance.SetUpShieldSkill();
        skillUI.GetComponent<Image>().sprite = skillIcon[1];
        skillUI.gameObject.SetActive(true);
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}
