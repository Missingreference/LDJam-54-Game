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


        //When Main Menu Button is clicked, do OnMainMenuButtonClick function
        backButton.onClick.AddListener(OnBackButtonClick);

        //When Done Button is clicked, do OnDoneButtonClick function
        startButton.onClick.AddListener(OnStartButtonClick);

        fadeInBlackImage = transform.Find("Fade In Black Image").GetComponent<Image>();

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


        

    }

    // Update is called once per frame
    void Update()
    {
        
        
        if (fadeTimer > 0)
        {
           
            fadeTimer -= Time.deltaTime;
            float fadePercent = 1f - (fadeTimer / fadeTime);
            fadeInBlackImage.color = Easing.Linear.InOut(Color.clear, Color.black, fadePercent);

            if(fadeTimer <= 0)
            {
                SceneManager.LoadScene("Game");
            }

        }


    }
}