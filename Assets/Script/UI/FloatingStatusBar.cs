using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingStatusBar : MonoBehaviour
{
    [SerializeField]
    Slider slider;
    [SerializeField]
    TextMeshProUGUI Text;
    [SerializeField] Image fill;

    float flashInterval = 0.5f;
    float flashTimer;
    bool flash;
    bool flashActivated;


    private void Awake()
    {
        flashTimer = flashInterval;
    }
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        if (!flashActivated) return;
        if (flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0)
            {
                Flash(flash);
                flashTimer = flashInterval;
            }
        }
    }

    public void UpdateStatusBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }
    public void ActivateFlash(bool active, float interval = 0.5f)
    {
        flashActivated = active;
        if (!active)
        {
            fill.color = Color.red;
            return;
        }
        flashInterval = interval;
    }
    void Flash(bool flash)
    {
        fill.color = flash ? Color.red : Color.white;
        this.flash = !flash;
    }

    public void UpdateText(string text)
    {
        if (text != null)
            Text.text = text;
    }
}
