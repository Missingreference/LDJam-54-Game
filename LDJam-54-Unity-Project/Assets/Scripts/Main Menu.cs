using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    Button startButton;
    Button quitButton;
    CharacterSelect characterSelect; 

    // Start is called before the first frame update
    void Start()
    {
        // Got Start Button transform
        startButton = transform.Find("Start Button").GetComponent<Button>();

        //Get Quit Button Transform
        quitButton = transform.Find("Quit Button").GetComponent<Button>();

        // Get character select script
        characterSelect = FindObjectOfType<CharacterSelect>(true);



        //When Quit Button is clicked, do OnQuitButtonClick function
        quitButton.onClick.AddListener(OnQuitButtonClick);



        //When Start Button is click, do OnStartButtonClick function
        startButton.onClick.AddListener(OnStartButtonClick);
    }

  
    void OnQuitButtonClick()
    {
        //quit application 
        Application.Quit(); 
    }

    void OnStartButtonClick()
    {
        //deactivate main menu canvas
        gameObject.SetActive(false);
        //activate character select canvas
        characterSelect.gameObject.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
