using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Elanetic.Tools;

public class GameOver : MonoBehaviour
{
    float animateTime = 1.0f;
    float animateTimer;
    Image gameOverBackgroundImage;
    CanvasGroup gameOverCanvasGroup;
    Color32 newColor = new Color32(63, 63, 63, 255);
    HUD hUD;

    // Start is called before the first frame update
    void Start()
    {
        // get game over background image
        gameOverBackgroundImage = transform.Find("Game Over Background").GetComponent<Image>();

        //get other game over visuals transforms through a canvas group so that they fade with background
        gameOverCanvasGroup = transform.Find("Game Over Background").Find("Image").GetComponent<CanvasGroup>();

        //get HUD transform
        hUD = FindObjectOfType<HUD>(true);




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
            
            animateTimer -= Time.deltaTime;
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
