using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Elanetic.Tools;


public class RewardScreen : MonoBehaviour
{

    Button confirmButton;
    Image rewardsBackground;
    const float animationTime = 1;
    float animationTimer;
    bool confirmClick = false;
    Color32 newColor = new Color32(63, 63, 63, 200);
    CanvasGroup rewardsImage;




    // Start is called before the first frame update
    void Start()
    {
        
        //get rewards menu background transform
        rewardsBackground = transform.Find("Rewards Background").GetComponent<Image>();

        //get other reward menu visuals transform through a canvas group so that they fade with background
        rewardsImage = transform.Find("Image").GetComponent<CanvasGroup>();

        //get confirm button transform
        confirmButton = transform.Find("Image").Find("Confirm Button").GetComponent<Button>();




        //when confirm button is clicked, do function
        confirmButton.onClick.AddListener(OnConfirmButtonClick);

        rewardsBackground.color = newColor;
        animationTimer = animationTime;

    }

    void OnConfirmButtonClick()
    {
        confirmClick = true;
        animationTimer = animationTime;
    }

    // Update is called once per frame
    void Update()
    {
        //when reward screen ope
        if (confirmClick == false && animationTimer >= 0f)
        {
            animationTimer -= Time.deltaTime;
            float animationPercent = 1f - (animationTimer / animationTime);
            rewardsBackground.color = Easing.Linear.InOut(Color.clear, newColor, animationPercent);
            rewardsImage.alpha = Easing.Linear.InOut(0, 1, animationPercent);
        }

        //when confirm is clicked, rewards menu and background fade away 
        if (confirmClick == true && animationTimer >= 0f)
        {
            animationTimer -= Time.deltaTime;
            float animationPercent = 1f - (animationTimer / animationTime);
            rewardsBackground.color = Easing.Linear.InOut(newColor, Color.clear, animationPercent);
            rewardsImage.alpha = Easing.Linear.InOut(1,0,animationPercent);

            //when animations are done, rewards menu canvas is set inactive 
            if (animationTimer <= 0)
            {
                gameObject.SetActive(false);
                confirmClick = false;
                animationTimer = animationTime;

            }
        }
    }
}
