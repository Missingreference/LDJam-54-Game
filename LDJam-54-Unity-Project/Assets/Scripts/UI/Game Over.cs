using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Elanetic.Tools;
using TMPro;

public class GameOver : MonoBehaviour
{
    float animateTime = 1.0f;
    float animateTimer;
    Image gameOverBackgroundImage;
    CanvasGroup gameOverCanvasGroup;
    Color32 newColor = new Color32(63, 63, 63, 255);
    HUD hUD;
    MusicSource musicSource;
    private GameDirector m_GameDirector;

    // Start is called before the first frame update
    void Start()
    {
        m_GameDirector = FindObjectOfType<GameDirector>();
        musicSource = FindObjectOfType<MusicSource>();

        musicSource.PlayGameOverMusic();

        // get game over background image
        gameOverBackgroundImage = transform.Find("Game Over Background").GetComponent<Image>();

        //get other game over visuals transforms through a canvas group so that they fade with background
        gameOverCanvasGroup = transform.Find("Game Over Background").Find("Image").GetComponent<CanvasGroup>();

        //get HUD transform
        hUD = FindObjectOfType<HUD>(true);

        Transform parentT = transform.Find("Game Over Background").Find("Image");
        parentT.Find("Wave Stat").GetComponent<TextMeshProUGUI>().text = "Waves Completed " + (m_GameDirector.currentStage - 1).ToString();

        float minuteFloat = Mathf.Floor(m_GameDirector.timePlayed / 60.0f);
        int minute = (int)minuteFloat;

        int second = Mathf.FloorToInt(m_GameDirector.timePlayed - (minuteFloat * 60.0f));
        TextMeshProUGUI timeStatText = parentT.Find("Time Stat").GetComponent<TextMeshProUGUI>();

        if(second < 10)
        {
            timeStatText.text = "Time Survived   " + minute.ToString() + ":0" + second.ToString();
        }
        else
        {
            timeStatText.text = "Time Survived   " + minute.ToString() + ":" + second.ToString();
        }

        parentT.Find("Enemy Stat").GetComponent<TextMeshProUGUI>().text = "Enemies Killed   " + (m_GameDirector.slimesKilled + m_GameDirector.shrubsKilled + m_GameDirector.potgoblinsKilled).ToString();

        parentT.Find("Overall Relics").GetComponent<TextMeshProUGUI>().text = "Relics Collected    " + (m_GameDirector.commonRelicsPickedUp + m_GameDirector.uncommonRelicsPickedUp + m_GameDirector.rareRelicsPickedUp  + m_GameDirector.legendaryRelicsPickedUp).ToString();

        parentT.Find("Common Relics").GetComponent<TextMeshProUGUI>().text = "Common Relics    " + (m_GameDirector.commonRelicsPickedUp).ToString();
        parentT.Find("Uncommon Relics").GetComponent<TextMeshProUGUI>().text = "Uncommon Relics    " + (m_GameDirector.uncommonRelicsPickedUp).ToString();
        parentT.Find("Rare Relics").GetComponent<TextMeshProUGUI>().text = "Rare Relics    " + (m_GameDirector.rareRelicsPickedUp).ToString();
        parentT.Find("Legendary Relics").GetComponent<TextMeshProUGUI>().text = "Legendary Relics    " + (m_GameDirector.legendaryRelicsPickedUp).ToString();



        //set game over background image to clear at beginning
        gameOverBackgroundImage.color = Color.clear;

        animateTimer = animateTime; 
    }
    
        

    // Update is called once per frame
    void Update()
    {
        //fade game over background and other visuals in at beginning
        if (animateTimer >= 0)
        {
            
            animateTimer -= Time.unscaledDeltaTime;
            float animatePercent = 1f - (animateTimer / animateTime);
            gameOverBackgroundImage.color = Easing.Linear.InOut(Color.clear, newColor, animatePercent);
            gameOverCanvasGroup.alpha = Easing.Linear.InOut(0, 1, animatePercent);

            //after game over menu has faded in, set HUD as inactive
            if(animateTimer <= 0)
            {
                hUD.gameObject.SetActive(false);

            }
        }
        
        
    }
}
