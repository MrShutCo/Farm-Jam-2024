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

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void UpdateStatusBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }

    public void UpdateText(string text)
    {
        if (text != null)
            Text.text = text;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
