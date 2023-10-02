using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Elanetic.Tools;

public class PauseMenu : MonoBehaviour
{

    
    Button resumeButton;
    const float startPosition = 425f;
    const float endPosition = 0f;
    const float animateDownTime = 1.0f;
    float animateDownTimer;
    RectTransform rectTransform;
    Image backGroundImage;
    bool resumeFunctionTrue;
    Color32 newColor = new Color32(63, 63, 63, 200);
    Slider pauseMenuVolumeSlider;
    


    // Start is called before the first frame update
    void Start()
    {
        //if (gameObject.SetActive(true))
        //{
        //    animateDownTime = 1.0f;
        //}


        //Get transform of pause menu image
        Transform pauseMenuTransform = transform.Find("Background").Find("Pause Menu");

        // Get rect transform of pause menu image
        rectTransform = (RectTransform)pauseMenuTransform;

        // set pause menu image rec transform starting position
        rectTransform.localPosition = new Vector3(0, startPosition, 0);

        //get pause menu background image
        backGroundImage = transform.Find("Background").GetComponent<Image>();

        //get transform of resume button
        resumeButton = transform.Find("Background").Find("Pause Menu").Find("Resume Button").GetComponent<Button>();

        //get transform of pause menu volume slider
        pauseMenuVolumeSlider = transform.Find("Background").Find("Pause Menu").Find("Volume Slider").GetComponent<Slider>();

        //set slider position to be in sync with volume float at beginning
        pauseMenuVolumeSlider.value = MusicSource.volume;
        //when volume slider is used, do function
        pauseMenuVolumeSlider.onValueChanged.AddListener(ChangeVolume);
        

        //when resume button is clicked, do OnResumeButtonClick function
        resumeButton.onClick.AddListener(OnResumeButtonClick);



        //set pause menu background image to clear at the beginning
        backGroundImage.color = Color.clear;

        animateDownTimer = animateDownTime;

        

        
    }

    void OnResumeButtonClick()
    {
        resumeFunctionTrue = true;
        
        animateDownTimer = animateDownTime;

        
    }

    
    void ChangeVolume(float value)
    {
        AudioListener.volume = pauseMenuVolumeSlider.value;
        MusicSource.volume = value;

    }

    // Update is called once per frame
    void Update()
    {
        //when pause menu opens, bring pause menu down and fade in back ground 
        if (animateDownTimer >= 0 && resumeFunctionTrue == false)
        {
            animateDownTimer -= Time.deltaTime;

            float percent = 1f - (animateDownTimer / animateDownTime);

            float easingPosition = Easing.Quadratic.InOut(startPosition, endPosition, percent);

            rectTransform.localPosition = new Vector3(0, easingPosition, 0);
            //rectTransform.localPosition = new Vector3(0, ((endPosition - startPosition)* percent) + startPosition, 0);

            backGroundImage.color = Easing.Linear.InOut(Color.clear, newColor, percent);
            

        }
        // when resume button is clicked, bring pause menu back up and fade out background 
        if(resumeFunctionTrue == true && animateDownTimer >= 0)
        {
            animateDownTimer -= Time.deltaTime;

            float percent = 1f - (animateDownTimer / animateDownTime);

            float easingPosition2 = Easing.Quadratic.InOut(endPosition, startPosition, percent);

            rectTransform.localPosition = new Vector3(0, easingPosition2, 0);

            backGroundImage.color = Easing.Linear.InOut(newColor, Color.clear, percent);

            if (animateDownTimer <= 0)
            {
                gameObject.SetActive(false);
                resumeFunctionTrue = false;
                animateDownTimer = animateDownTime;
            }
            
        }



    }
}
