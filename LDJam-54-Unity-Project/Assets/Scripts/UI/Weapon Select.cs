using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Elanetic.Tools;

public class WeaponSelect : MonoBehaviour
{

    Button startButton;
    Button backButton;
    MainMenu mainMenu;
    Image fadeInBlackImage;
    float fadeTime;
    float fadeTimer;
    bool startButtonClicked = false;



    // Start is called before the first frame update
    void Start()
    {
        //Get Main Menu script
        mainMenu = FindObjectOfType<MainMenu>(true);

        //Get Done Button transform
        startButton = transform.Find("Start Button").GetComponent<Button>();

        //Get Main Menu Button transform
        backButton = transform.Find("Back Button").GetComponent<Button>();

        //Get FadeInBlack image transform
        fadeInBlackImage = transform.Find("Fade In Black Image").GetComponent<Image>();




        //When Main Menu Button is clicked, do OnMainMenuButtonClick function
        backButton.onClick.AddListener(OnBackButtonClick);

        //When Done Button is clicked, do OnDoneButtonClick function
        startButton.onClick.AddListener(OnStartButtonClick);

        

        //fadinblack image starts as clear 
        fadeInBlackImage.color = Color.clear;

        
        
        
    }

    void OnBackButtonClick()
    {
        //Character Select Canvas is set inactive
        gameObject.SetActive(false);
        //Main menu canvas is set active
        mainMenu.gameObject.SetActive(true);

        
    }

    void OnStartButtonClick()
    {
        fadeInBlackImage.gameObject.SetActive(true);

        fadeTime = 2.0f;
        fadeTimer = fadeTime;

        startButtonClicked = true;


    }

    // Update is called once per frame
    void Update()
    {
        
        //if start button is clicked, fade to black
        if (fadeTimer >= 0 && startButtonClicked == true)
        {
           
            fadeTimer -= Time.unscaledDeltaTime;
            float fadePercent = 1f - (fadeTimer / fadeTime);
            fadeInBlackImage.color = Easing.Linear.InOut(Color.clear, Color.black, fadePercent);

            //when fading animation is done, go to game scene
            if(fadeTimer <= 0)
            {
                SceneManager.LoadScene("Game");
            }

        }


    }
}
