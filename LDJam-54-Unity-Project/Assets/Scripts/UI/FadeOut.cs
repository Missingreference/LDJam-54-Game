using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Elanetic.Tools;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour
{

    float fadeTimer;
    float fadeTime = 2f;
    Image fadeOutBlackImage;
    bool quitButtonClickTrue = false;
    PauseMenu pauseMenu;
    Button pauseMenuQuitButton;
    GameOver gameOverMenu;
    Button gameOverQuitButton;
    
    // Start is called before the first frame update
    void Start()
    {
        //get transform of fade to clear image
        fadeOutBlackImage = transform.Find("Fade Out Black Image").GetComponent<Image>();

        //get quit button transform in the pause menu
        pauseMenu = FindObjectOfType<PauseMenu>(true);
        pauseMenuQuitButton = pauseMenu.transform.Find("Background").Find("Pause Menu").Find("Quit Button").GetComponent<Button>();

        //get quit button transform in the game over menu
        gameOverMenu = FindObjectOfType<GameOver>(true);
        gameOverQuitButton = gameOverMenu.transform.Find("Game Over Background").Find("Image").Find("Quit Button").GetComponent<Button>();



        //when pause menu quit button is clicked, do function OnPauseMenuQuitButtonClick
        pauseMenuQuitButton.onClick.AddListener(OnPauseMenuQuitButtonClick);

        //when game over quit button is clicked, do function
        gameOverQuitButton.onClick.AddListener(OnGameOverQuitButtonClick);



        fadeTimer = fadeTime;
    }



    void OnPauseMenuQuitButtonClick()
    {
        if(pauseMenu.hiding) return;

        fadeOutBlackImage.gameObject.SetActive(true);
        quitButtonClickTrue = true;
        fadeTime = 2f;
        fadeTimer = fadeTime;
       
    }

    void OnGameOverQuitButtonClick()
    {
        if(pauseMenu.hiding) return;

        fadeOutBlackImage.gameObject.SetActive(true);
        quitButtonClickTrue = true;
        fadeTime = 2f;
        fadeTimer = fadeTime;
    }

    // Update is called once per frame
    void Update()
    {
        //fade into the beginning of game scene
        if (quitButtonClickTrue == false && fadeTimer >= 0)
        {

            fadeTimer -= Time.unscaledDeltaTime;
            float fadePercent = 1f - (fadeTimer / fadeTime);
            fadeOutBlackImage.color = Easing.Linear.InOut(Color.black, Color.clear, fadePercent);
            
        }
        //if the quit button is clicked, fade to black  
        else if (quitButtonClickTrue == true && fadeTimer >= 0)
        {
            fadeTimer -= Time.unscaledDeltaTime;
            float fadePercent = 1f - (fadeTimer / fadeTime);
            fadeOutBlackImage.color = Easing.Linear.InOut(Color.clear, Color.black, fadePercent);

            //once fade is done, take us to main menu scene
            if (fadeTimer <= 0)
            {
                
                pauseMenu.gameObject.SetActive(false);
                gameOverMenu.gameObject.SetActive(false);
                Time.timeScale = 1.0f;
                SceneManager.LoadScene("Main Menu");
            }

        }
        //once fade is complete, set the fadeing image inactive 
        else
        {
            fadeOutBlackImage.gameObject.SetActive(false);
        }

        
    }
}
