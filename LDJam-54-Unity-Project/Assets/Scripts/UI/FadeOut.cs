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
    Button quitButton;
    
    // Start is called before the first frame update
    void Start()
    {
        //get transform of fade to clear image
        fadeOutBlackImage = transform.Find("Fade Out Black Image").GetComponent<Image>();

        

        fadeTimer = fadeTime;

        //get quit button transform in the pause menu
        pauseMenu = FindObjectOfType<PauseMenu>(true);
        quitButton = pauseMenu.transform.Find("Background").Find("Pause Menu").Find("Quit Button").GetComponent<Button>();

        //when quit button i clicked, do function OnPauseMenuQuitButtonClick
        quitButton.onClick.AddListener(OnPauseMenuQuitButtonClick);
    }

    void OnPauseMenuQuitButtonClick()
    {
        fadeOutBlackImage.gameObject.SetActive(true);
        quitButtonClickTrue = true;
        fadeTime = 2f;
        fadeTimer = fadeTime;
        //goToMainMenuTrue = true;

    }

    // Update is called once per frame
    void Update()
    {
        //if time is not 0, fade
        if (quitButtonClickTrue == false && fadeTimer >= 0)
        {

            fadeTimer -= Time.deltaTime;
            float fadePercent = 1f - (fadeTimer / fadeTime);
            fadeOutBlackImage.color = Easing.Linear.InOut(Color.black, Color.clear, fadePercent);
            
        }
        else if (quitButtonClickTrue == true && fadeTimer >= 0)
        {
            fadeTimer -= Time.deltaTime;
            float fadePercent = 1f - (fadeTimer / fadeTime);
            fadeOutBlackImage.color = Easing.Linear.InOut(Color.clear, Color.black, fadePercent);

            if (fadeTimer <= 0)
            {
                SceneManager.LoadScene("Main Menu");
            }

        }
        else
        {
            fadeOutBlackImage.gameObject.SetActive(false);
        }

        
    }
}
