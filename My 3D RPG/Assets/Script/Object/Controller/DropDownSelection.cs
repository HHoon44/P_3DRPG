using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropDownSelection : MonoBehaviour
{
    private Dropdown dropDown;

    void Start()
    {
        dropDown = GetComponent<Dropdown>();
    }

    public void OnChange()
    {
        QualitySettings.SetQualityLevel(dropDown.value, false);
    }
}
