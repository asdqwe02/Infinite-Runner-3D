using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SkillCoolDownIcon : MonoBehaviour
{
    [SerializeField] private Image cooldownImage;
    [SerializeField] private TMP_Text cooldownText;
    public float cooldownTime;
    public float cooldownTimer;
    // Start is called before the first frame update
    void Start()
    {
        cooldownText.gameObject.SetActive(false);
        cooldownImage.fillAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !PlayerController.instance.skillCD)
        {
            cooldownText.gameObject.SetActive(true);
            cooldownImage.fillAmount = 1f;
            cooldownTime = PlayerController.instance.skillCDTimeTotal;
            cooldownTimer = cooldownTime;
        }
        if (PlayerController.instance.skillCD)
        {
            ApplyCoolDownEffect();
        }
    }
    public void ApplyCoolDownEffect()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            cooldownText.gameObject.SetActive(false);
            cooldownImage.fillAmount = 0f;
        }
        else
        {
            cooldownText.text = cooldownTimer.ToString("F1");
            cooldownImage.fillAmount = cooldownTimer / cooldownTime;
        }
    }

}
