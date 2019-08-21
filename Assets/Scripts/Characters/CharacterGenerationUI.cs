using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterGenerationUI : MonoBehaviour
{
    public Toggle statToggle;

    private Image toggleImage;

    public Color selectedStatColor = Color.red;

    void Awake()
    {
        toggleImage = GetComponent<Image>();
    }

    public void ChangeColor()
    {
        if (statToggle.isOn)
        {
            toggleImage.color = selectedStatColor;
        }
    }

    //Check if any button in array is clicked


    //Loop through button array and deselect each button

    //Highlight the selected button. 

    
}
