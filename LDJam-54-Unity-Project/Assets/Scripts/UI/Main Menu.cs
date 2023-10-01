using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    Button playButton;
    Button quitButton;
    WeaponSelect weaponSelect; 

    // Start is called before the first frame update
    void Start()
    {
        // Got Start Button transform
        playButton = transform.Find("Play Button").GetComponent<Button>();

        //Get Quit Button Transform
        quitButton = transform.Find("Quit Button").GetComponent<Button>();

        // Get character select script
        weaponSelect = FindObjectOfType<WeaponSelect>(true);



        //When Quit Button is clicked, do OnQuitButtonClick function
        quitButton.onClick.AddListener(OnQuitButtonClick);



        //When Start Button is click, do OnStartButtonClick function
        playButton.onClick.AddListener(OnPlayButtonClick);
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
        
    }
}
