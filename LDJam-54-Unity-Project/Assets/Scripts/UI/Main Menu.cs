using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Elanetic.Tools;

public class MainMenu : MonoBehaviour
{

    Button playButton;
    Button quitButton;
    WeaponSelect weaponSelect;
    float animateTime = 0.5f;
    float animateTimer;
    Image blackFadeOut;
    Slider mainMenuVolumeSlider;
    

    
   
  

    // Start is called before the first frame update
    void Start()
    {


        // Got Start Button transform
        playButton = transform.Find("Play Button").GetComponent<Button>();

        //Get Quit Button Transform
        quitButton = transform.Find("Quit Button").GetComponent<Button>();

        // Get character select script
        weaponSelect = FindObjectOfType<WeaponSelect>(true);

        //Get fading image transform
        blackFadeOut = transform.Find("Black Fade Out").GetComponent<Image>();



        //Get transform of main menu volume slider
        mainMenuVolumeSlider = transform.Find("Volume Slider").GetComponent<Slider>();



        //When Quit Button is clicked, do OnQuitButtonClick function
        quitButton.onClick.AddListener(OnQuitButtonClick);


        //When Start Button is click, do OnStartButtonClick function
        playButton.onClick.AddListener(OnPlayButtonClick);



        //when volume slider is used, do function
        mainMenuVolumeSlider.onValueChanged.AddListener(ChangeVolume);



        //set volume at beginning to 0.5
        mainMenuVolumeSlider.value = MusicSource.volume;

        //set intital fadig image color to black
        blackFadeOut.color = Color.black;

        animateTimer = animateTime; 
    }

  
    void OnQuitButtonClick()
    {
        //quit application 
        Application.Quit(); 
    }

    void OnPlayButtonClick()
    {
        //deactivate main menu canvas
        gameObject.SetActive(false);
        //activate character select canvas
        weaponSelect.gameObject.SetActive(true);
        
    }

    public void ChangeVolume(float value)
    {

        AudioListener.volume = mainMenuVolumeSlider.value;
        MusicSource.volume = value;

    }

    // Update is called once per frame
    void Update()
    {
        //make fading image go from black to clear at start of main menu scene
        if (animateTimer >= 0)
        {
            animateTimer -= Time.deltaTime;

            float animatePercent = 1 - (animateTimer / animateTime);
            
            blackFadeOut.color = Easing.Linear.InOut(Color.black, Color.clear, animatePercent);
        }
        else
        {
            //make fading image inactive so we can still click buttons after it is done animating 
            blackFadeOut.gameObject.SetActive(false);
        }

        
    }
}
