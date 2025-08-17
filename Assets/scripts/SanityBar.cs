using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SanityBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxSanity(float max)
    {
        slider.maxValue = max;
        slider.value = max;
    }

    public void SetSanity(float sanity)
    {
        if (sanity <= 0.0f)
        {
            slider.value = 0.0f;
        } 
        else
        {
            slider.value = sanity;
        }
        
    }
}
