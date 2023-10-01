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
    float animateTime = 2f;
    float animateTimer;
    Image blackFadeOut;
   
  

    // Start is called before the first frame update
    void Start()
    {


        // Got Start Button transform
        playButton = transform.Find("Play Button").GetComponent<Button>();

        //Get Quit Button Transform
        quitButton = transform.Find("Quit Button").GetComponent<Button>();

        // Get character select script
        weaponSelect = FindObjectOfType<WeaponSelect>(true);

        blackFadeOut = transform.Find("Black Fade Out").GetComponent<Image>();

        blackFadeOut.color = Color.black;

        //When Quit Button is clicked, do OnQuitButtonClick function
        quitButton.onClick.AddListener(OnQuitButtonClick);



        //When Start Button is click, do OnStartButtonClick function
        playButton.onClick.AddListener(OnPlayButtonClick);

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

    // Update is called once per frame
    void Update()
    {
        if (animateTimer >= 0)
        {
            animateTimer -= Time.deltaTime;

            float animatePercent = 1 - (animateTimer / animateTime);
            
            blackFadeOut.color = Easing.Linear.InOut(Color.black, Color.clear, animatePercent);
        }
        else
        {
            blackFadeOut.gameObject.SetActive(false);
        }

    }
}
