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
    float startPosition = 425f;
    float endPosition = 0f;
    float animateTime = 1.0f;
    float animateTimer;
    RectTransform rectTransform;
    Image backGroundImage;
    


    // Start is called before the first frame update
    void Start()
    {




        Transform pauseMenuTransform = transform.Find("Background").Find("Pause Menu");

        rectTransform = (RectTransform)pauseMenuTransform;
        rectTransform.localPosition = new Vector3(0, startPosition, 0);


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

        animateTimer = animateTime;

        
    }

    void OnResumeButtonClick()
    {
        gameObject.SetActive(false);
    }

    void OnQuitButtonClick()
    {
        SceneManager.LoadScene("Main Menu");

    }

    // Update is called once per frame
    void Update()
    {
        if (animateTimer >= 0)
        {
            animateTimer -= Time.deltaTime;
            float percent = 1f - (animateTimer / animateTime);
            float easingPosition = Easing.Quadratic.InOut(startPosition, endPosition, percent);
            rectTransform.localPosition = new Vector3(0, easingPosition, 0);
            //rectTransform.localPosition = new Vector3(0, ((endPosition - startPosition)* percent) + startPosition, 0);

            
            Color32 newColor = new Color32(63, 63, 63, 200);
            Color anotherColor = newColor;

            backGroundImage.color = Easing.Linear.InOut(Color.clear, anotherColor, percent);

            Debug.Log(backGroundImage.color);


            

        }



    }
}
