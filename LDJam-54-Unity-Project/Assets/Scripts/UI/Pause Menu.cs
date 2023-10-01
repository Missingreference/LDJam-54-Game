using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Elanetic.Tools;

public class PauseMenu : MonoBehaviour
{

    Button quitButton;
    Button resumeButton;
    float startDownPosition = 425f;
    float endDownPosition = 0f;
    float animateDownTime = 1.0f;
    float animateDownTimer;
    float animateUpTime;
    float animateUpTimer;
    RectTransform rectTransform;
    Image backGroundImage;
    Image fadeIntoBlack;
    bool resumeFunctionTrue;
    Color32 newColor = new Color32(63, 63, 63, 200);
    


    // Start is called before the first frame update
    void Start()
    {




        Transform pauseMenuTransform = transform.Find("Background").Find("Pause Menu");

        rectTransform = (RectTransform)pauseMenuTransform;
        rectTransform.localPosition = new Vector3(0, startDownPosition, 0);


        backGroundImage = transform.Find("Background").GetComponent<Image>();

        backGroundImage.color = Color.clear;


        //get transform of resume button
        resumeButton = transform.Find("Background").Find("Pause Menu").Find("Resume Button").GetComponent<Button>();

        //get transform of quit button
        quitButton = transform.Find("Background").Find("Pause Menu").Find("Quit Button").GetComponent<Button>();

        //when resume button is clicked, do OnResumeButtonClick function
        resumeButton.onClick.AddListener(OnResumeButtonClick);

        //when quit button is clicked, do OnQuitButtonClick Function
        quitButton.onClick.AddListener(OnQuitButtonClick);

        animateDownTimer = animateDownTime;

        fadeIntoBlack = FindObjectOfType<FadeOut>().transform.Find("Fade Out Black Image").GetComponent<Image>();

        
    }

    void OnResumeButtonClick()
    {
        resumeFunctionTrue = true;
        animateDownTime = 1f;
        animateDownTimer = animateDownTime;

        //gameObject.SetActive(false);
    }

    void OnQuitButtonClick()
    {
        //resumeFunctionTrue = true; 
        //SceneManager.LoadScene("Main Menu");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (animateDownTimer >= 0 && resumeFunctionTrue == false)
        {
            animateDownTimer -= Time.deltaTime;

            float percent = 1f - (animateDownTimer / animateDownTime);

            float easingPosition = Easing.Quadratic.InOut(startDownPosition, endDownPosition, percent);

            rectTransform.localPosition = new Vector3(0, easingPosition, 0);
            //rectTransform.localPosition = new Vector3(0, ((endPosition - startPosition)* percent) + startPosition, 0);

            
            
            

            backGroundImage.color = Easing.Linear.InOut(Color.clear, newColor, percent);
            

        }
        
        if(resumeFunctionTrue == true && animateDownTimer >= 0)
        {
            animateDownTimer -= Time.deltaTime;

            float percent = 1f - (animateDownTimer / animateDownTime);

            float easingPosition2 = Easing.Quadratic.InOut(endDownPosition, startDownPosition, percent);

            rectTransform.localPosition = new Vector3(0, easingPosition2, 0);

            backGroundImage.color = Easing.Linear.InOut(newColor, Color.clear, percent);

            
        }



    }
}
