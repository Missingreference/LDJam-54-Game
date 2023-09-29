using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        // Got Start Button transform
        startButton = transform.Find("Start Button").GetComponent<Button>();

        //When Start Button is click, do OnStartButtonClick function
        startButton.onClick.AddListener(OnStartButtonClick);
    }


    void OnStartButtonClick()
    {
        //Load Game scene
        SceneManager.LoadScene("Game");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
