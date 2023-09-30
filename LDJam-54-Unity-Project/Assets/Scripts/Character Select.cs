using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{

    Button readyButton;
    Button backButton;
    MainMenu mainMenu;

    // Start is called before the first frame update
    void Start()
    {
        //Get Main Menu script
        mainMenu = FindObjectOfType<MainMenu>(true);

        //Get Done Button transform
        readyButton = transform.Find("Ready Button").GetComponent<Button>();

        //Get Main Menu Button transform
        backButton = transform.Find("Back Button").GetComponent<Button>();

        //When Main Menu Button is clicked, do OnMainMenuButtonClick function
        backButton.onClick.AddListener(OnBackButtonClick);

        //When Done Button is clicked, do OnDoneButtonClick function
        readyButton.onClick.AddListener(OnReadyButtonClick);
    }

    void OnBackButtonClick()
    {
        //Character Select Canvas is set inactive
        gameObject.SetActive(false);
        //Main menu canvas is set active
        mainMenu.gameObject.SetActive(true);
    }

    void OnReadyButtonClick()
    {
        //Load game scene
        SceneManager.LoadScene("Game");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
