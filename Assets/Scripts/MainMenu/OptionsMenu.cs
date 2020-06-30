using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    private float volumeLocal; // Volume from 0.0 - 100.0
    private int PPOnLocal; // 0 or 1
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle PPToggle;
    [SerializeField] private GameObject previousButtonObject, previousMenu, currentMenu, defaultSelectableObject, MainMenuVFXObject;
    [SerializeField] private AK.Wwise.RTPC masterVolume;
    [SerializeField] private float conversionFactor = 2;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        // Select default button  
        /*Selectable defaultButton = defaultSelectableObject.GetComponent<Slider>();
        defaultButton.Select();
        defaultButton.OnSelect(null);*/

        // Get the values for local variables 
        volumeLocal = PlayerPrefs.GetFloat("volume");
        PPOnLocal = PlayerPrefs.GetInt("PostProcessing");

        // Put those values into the menu parts 
        // Volume slider
        volumeSlider.value = volumeLocal * conversionFactor;
        // Post Processing check box 
        PPToggle.SetIsOnWithoutNotify(Convert.ToBoolean(PPOnLocal));
    }


    public void SetVolume(float uselessValue)
    {
        float volume = volumeSlider.value / conversionFactor;
        volumeLocal = volume;
        PlayerPrefs.SetFloat("volume", volume);

        masterVolume.SetGlobalValue(volume); // For WWise ? 
    }


    public void SetPPOnOff(int uselessValue)
    {
        // Switch value 
        if (PPOnLocal == 1)
        {
            PPOnLocal = 0;
            PlayerPrefs.SetInt("PostProcessing", PPOnLocal);
        }
        else
        {
            PPOnLocal = 1;
            PlayerPrefs.SetInt("PostProcessing", PPOnLocal);
        }

        Debug.Log("New value for PPOn: " + Convert.ToBoolean(PPOnLocal).ToString());

        // Update VFX for main menu 
        MainMenuVFXObject.SetActive(Convert.ToBoolean(PlayerPrefs.GetInt("PostProcessing")));
    }


    public void CloseOptionsMenu()
    {
        //previousMenu.SetActive(true);

        // Select default button again 
        Selectable previousDefaultButton = previousButtonObject.GetComponent<Button>();
        previousDefaultButton.Select();
        previousDefaultButton.OnSelect(null);

        //currentMenu.SetActive(false);
    }
}
